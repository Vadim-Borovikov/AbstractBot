using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Models;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Modules;

[PublicAPI]
public class UpdateSender : IUpdateSender
{
    public KeyboardProvider DefaultKeyboardProvider { get; set; } = KeyboardProvider.Remove;

    public UpdateSender(TelegramBotClient client, IFileStorage fileStorage, ICooldown cooldown, LoggerExtended logger)
    {
        _client = client;
        _fileStorage = fileStorage;
        _cooldown = cooldown;
        _logger = logger;
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, KeyboardProvider? keyboardProvider = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        LinkPreviewOptions? linkPreviewOptions = null, int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        keyboardProvider ??= DefaultKeyboardProvider;
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.SendText, data: text);
        return _client.SendMessage(chat.Id, text, parseMode, replyParameters, keyboardProvider.Keyboard,
            linkPreviewOptions, messageThreadId, entities, disableNotification, protectContent, messageEffectId,
            businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(Chat chat, int messageId, string text,
        ParseMode parseMode = ParseMode.None, IEnumerable<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null, InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null, CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.EditText, messageId, text);
        return _client.EditMessageText(chat.Id, messageId, text, parseMode, entities, linkPreviewOptions, replyMarkup,
            businessConnectionId, cancellationToken);
    }

    public async Task<Message> EditMessageMediaAsync(Chat chat, int messageId, string path,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        InputFile? photo = _fileStorage.TryGetInputFileId(path);
        if (photo is not null)
        {
            return await EditMessageMediaAsync(chat, messageId, photo, replyMarkup, businessConnectionId,
                cancellationToken);
        }

        await using (FileStream stream = File.OpenRead(path))
        {
            photo = stream.ToInputFileStream();
            Message message = await EditMessageMediaAsync(chat, messageId, photo, replyMarkup, businessConnectionId,
                cancellationToken);

            PhotoSize? file = message.Photo.Largest();
            if (file is not null)
            {
                _fileStorage.TryAdd(path, file);
            }

            return message;
        }
    }

    public Task<Message> EditMessageMediaAsync(Chat chat, int messageId, InputFile file,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        InputMediaPhoto media = new(file);
        return EditMessageMediaAsync(chat, messageId, media, replyMarkup, businessConnectionId, cancellationToken);
    }

    public Task<Message> EditMessageMediaAsync(Chat chat, int messageId, InputMedia media,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.EditMedia, messageId);
        return _client.EditMessageMedia(chat.Id, messageId, media, replyMarkup, businessConnectionId,
            cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(Chat chat, int messageId, string? caption,
        ParseMode parseMode = ParseMode.None, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null, CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.EditText, messageId);
        return _client.EditMessageCaption(chat.Id, messageId, caption, parseMode, captionEntities,
            showCaptionAboveMedia, replyMarkup, businessConnectionId, cancellationToken);
    }

    public Task DeleteMessageAsync(Chat chat, int messageId, CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.Delete, messageId);
        return _client.DeleteMessage(chat.Id, messageId, cancellationToken);
    }

    public Task<Message> ForwardMessageAsync(Chat chat, ChatId fromChatId, int messageId, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, int? videoStartTimestamp = null,
        CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.Forward, data: $"message {messageId} from {fromChatId}");
        return _client.ForwardMessage(chat.Id, fromChatId, messageId, messageThreadId, disableNotification,
            protectContent, videoStartTimestamp, cancellationToken);
    }

    public async Task<Message[]> SendMediaGroupAsync(Chat chat, IList<string> paths, string? caption = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        List<FileStream> streams = new();
        List<InputFile> inputFiles = new();

        foreach (string path in paths)
        {
            InputFile? photo = _fileStorage.TryGetInputFileId(path);
            if (photo is null)
            {
                FileStream stream = File.OpenRead(path);
                streams.Add(stream);
                photo = stream.ToInputFileStream();
            }
            inputFiles.Add(photo);
        }

        Message[] messages = await SendMediaGroupAsync(chat, inputFiles, caption, parseMode, replyParameters,
            messageThreadId, disableNotification, protectContent, messageEffectId, businessConnectionId,
            allowPaidBroadcast, cancellationToken);

        foreach (FileStream stream in streams)
        {
            await stream.DisposeAsync();
        }

        if (paths.Count == messages.Length)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                PhotoSize? file = messages[i].Photo.Largest();
                if (file is not null)
                {
                    _fileStorage.TryAdd(path, file);
                }
            }
        }
        else
        {
            _logger.LogError("Wrong MediaGroup size", $"Recieved {messages.Length} after {paths.Count} paths");
        }

        return messages;
    }

    public Task<Message[]> SendMediaGroupAsync(Chat chat, IList<InputFile> inputFiles, string? caption = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        List<InputMediaPhoto> media = new();
        for (int i = 0; i < inputFiles.Count; ++i)
        {
            bool addCaption = i == (inputFiles.Count - 1);
            InputMediaPhoto photo = new(inputFiles[i])
            {
                Caption = addCaption ? caption : null,
                ParseMode = parseMode
            };
            media.Add(photo);
        }

        return SendMediaGroupAsync(chat, media, replyParameters, messageThreadId, disableNotification, protectContent,
            messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public Task<Message[]> SendMediaGroupAsync(Chat chat, IEnumerable<IAlbumInputMedia> media,
        ReplyParameters? replyParameters = null, int? messageThreadId = null, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        List<IAlbumInputMedia> all = new(media);
        string captions = string.Join(", ", all.OfType<InputMedia>().Select(m => m.Caption).SkipNulls());
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.SendFiles, data: captions);

        return _client.SendMediaGroup(chat.Id, all, replyParameters, messageThreadId, disableNotification,
            protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public async Task<Message> SendPhotoAsync(Chat chat, string path, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, bool hasSpoiler = false, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default)
    {
        InputFile? photo = _fileStorage.TryGetInputFileId(path);
        if (photo is not null)
        {
            return await SendPhotoAsync(chat, photo, keyboardProvider, caption, parseMode, replyParameters,
                messageThreadId, captionEntities, showCaptionAboveMedia, hasSpoiler, disableNotification,
                protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
        }

        await using (FileStream stream = File.OpenRead(path))
        {
            photo = stream.ToInputFileStream();
            Message message = await SendPhotoAsync(chat, photo, keyboardProvider, caption, parseMode, replyParameters,
                messageThreadId, captionEntities, showCaptionAboveMedia, hasSpoiler, disableNotification,
                protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);

            PhotoSize? file = message.Photo.Largest();
            if (file is not null)
            {
                _fileStorage.TryAdd(path, file);
            }

            return message;
        }
    }

    public Task<Message> SendPhotoAsync(Chat chat, InputFile photo, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, bool hasSpoiler = false, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default)
    {
        keyboardProvider ??= DefaultKeyboardProvider;
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.SendPhoto, data: caption);

        return _client.SendPhoto(chat.Id, photo, caption, parseMode, replyParameters, keyboardProvider.Keyboard,
            messageThreadId, captionEntities, showCaptionAboveMedia, hasSpoiler, disableNotification, protectContent,
            messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public async Task<Message> SendDocumentAsync(Chat chat, string path, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        InputFile? thumbnail = null, int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool disableContentTypeDetection = false, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        InputFile? file = _fileStorage.TryGetInputFileId(path);
        if (file is not null)
        {
            return await SendDocumentAsync(chat, file, keyboardProvider, caption, parseMode, replyParameters,
                thumbnail, messageThreadId, captionEntities, disableContentTypeDetection, disableNotification,
                protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
        }

        await using (FileStream stream = File.OpenRead(path))
        {
            file = stream.ToInputFileStream();
            Message message = await SendDocumentAsync(chat, file, keyboardProvider, caption, parseMode,
                replyParameters, thumbnail, messageThreadId, captionEntities, disableContentTypeDetection,
                disableNotification, protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast,
                cancellationToken);

            if (message.Document is not null)
            {
                _fileStorage.TryAdd(path, message.Document);
            }

            return message;
        }
    }

    public Task<Message> SendDocumentAsync(Chat chat, InputFile document, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        InputFile? thumbnail = null, int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool disableContentTypeDetection = false, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        keyboardProvider ??= DefaultKeyboardProvider;
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.SendPhoto, data: caption);

        return _client.SendDocument(chat.Id, document, caption, parseMode, replyParameters, keyboardProvider.Keyboard,
            thumbnail, messageThreadId, captionEntities, disableContentTypeDetection, disableNotification, protectContent,
            messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputFile sticker, ReplyParameters? replyParameters = null,
        KeyboardProvider? keyboardProvider = null, int? messageThreadId = null, string? emoji = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        keyboardProvider ??= DefaultKeyboardProvider;
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.SendSticker);

        return _client.SendSticker(chat.Id, sticker, replyParameters, keyboardProvider.Keyboard, emoji, messageThreadId,
            disableNotification, protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast,
            cancellationToken);
    }

    public Task PinChatMessageAsync(Chat chat, int messageId, bool disableNotification = false,
        string? businessConnectionId = null, CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.Pin, messageId);
        return _client.PinChatMessage(chat.Id, messageId, disableNotification, businessConnectionId, cancellationToken);
    }

    public Task UnpinChatMessageAsync(Chat chat, int? messageId = null, string? businessConnectionId = null,
        CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.Unpin, messageId);
        return _client.UnpinChatMessage(chat.Id, messageId, businessConnectionId, cancellationToken);
    }

    public Task UnpinAllChatMessagesAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.UnpinAll);
        return _client.UnpinAllChatMessages(chat.Id, cancellationToken);
    }

    public Task SendInvoiceAsync(Chat chat, string title, string description, string payload, string currency,
        IEnumerable<LabeledPrice> prices, string? providerToken = default, string? providerData = default,
        int? maxTipAmount = default, IEnumerable<int>? suggestedTipAmounts = default, string? photoUrl = default,
        int? photoSize = default, int? photoWidth = default, int? photoHeight = default, bool needName = default,
        bool needPhoneNumber = default, bool needEmail = default, bool needShippingAddress = default,
        bool sendPhoneNumberToProvider = default, bool sendEmailToProvider = default, bool isFlexible = default,
        ReplyParameters? replyParameters = default, InlineKeyboardMarkup? replyMarkup = default,
        string? startParameter = default, int? messageThreadId = default, bool disableNotification = default,
        bool protectContent = default, string? messageEffectId = default, bool allowPaidBroadcast = default,
        CancellationToken cancellationToken = default)
    {
        _cooldown.DelayIfNeeded(chat, cancellationToken);
        _logger.LogUpdate(chat, LoggerExtended.UpdateType.SendInvoice, data: title);
        return _client.SendInvoice(chat.Id, title, description, payload, currency, prices, providerToken, providerData,
            maxTipAmount, suggestedTipAmounts, photoUrl, photoSize, photoWidth, photoHeight, needName, needPhoneNumber,
            needEmail, needShippingAddress, sendPhoneNumberToProvider, sendEmailToProvider, isFlexible,
            replyParameters, replyMarkup, startParameter, messageThreadId, disableNotification, protectContent,
            messageEffectId, allowPaidBroadcast, cancellationToken);
    }

    public Task AnswerCallbackQueryAsync(string callbackQueryId, string? text = null, bool showAlert = false,
        string? url = null, int? cacheTime = null, CancellationToken cancellationToken = default)
    {
        return _client.AnswerCallbackQuery(callbackQueryId, text, showAlert, url, cacheTime, cancellationToken);
    }

    private readonly TelegramBotClient _client;
    private readonly IFileStorage _fileStorage;
    private readonly ICooldown _cooldown;
    private readonly LoggerExtended _logger;
}