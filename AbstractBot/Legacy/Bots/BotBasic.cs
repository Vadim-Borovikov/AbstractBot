using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Extensions;
using AbstractBot.Legacy.Logging;
using AbstractBot.Legacy.Operations;
using AbstractBot.Legacy.Operations.Commands;
using AbstractBot.Servicies;
using AbstractBot.Utilities.Ngrok;
using GryphonUtilities;
using GryphonUtilities.Extensions;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public abstract class BotBasic
{
    public readonly TelegramBotClient Client;
    public readonly ConfigBasic ConfigBasic;
    public readonly Clock Clock;
    public readonly Logger Logger;
    public readonly SerializerOptionsProvider JsonSerializerOptionsProvider;

    protected internal readonly List<OperationBasic> Operations;

    protected readonly Dictionary<long, AccessData> Accesses;
    protected readonly InputFileId DontUnderstandSticker;
    protected readonly InputFileId ForbiddenSticker;

    protected internal virtual KeyboardProvider? StartKeyboardProvider => null;

    public readonly User Self;

    // before BotBasic creation
    private static async Task<string> GetHostAsync(JsonSerializerOptions options, string? defaultHost = null)
    {
        if (string.IsNullOrWhiteSpace(defaultHost))
        {
            return await Manager.GetHostAsync(options);
        }
        return defaultHost;
    }

    // Also:
    //  TelegramBotClient client = new(configBasic.Token);
    //  User self = await client.GetMe(cancellationToken);

    protected BotBasic(ConfigBasic configBasic, TelegramBotClient client, User self, string host)
    {
        ConfigBasic = configBasic;
        Client = client;
        Self = self;

        Help help = new(ConfigBasic);
        Operations = new List<OperationBasic>
        {
            help
        };

        DontUnderstandSticker = new InputFileId(ConfigBasic.DontUnderstandStickerFileId);
        ForbiddenSticker = new InputFileId(ConfigBasic.ForbiddenStickerFileId);

        Clock = new Clock(ConfigBasic.SystemTimeZoneId);
        Logger = new Logger(Clock);
        _ticker = new Ticker(Logger);

        JsonSerializerOptionsProvider = new SerializerOptionsProvider(Clock);

        _sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / configBasic.UpdatesPerSecondLimitPrivate);
        _sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / configBasic.UpdatesPerSecondLimitGlobal);
        _sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / configBasic.UpdatesPerMinuteLimitGroup);
        Accesses = GetAccesses();

        _connection = new ConnectionService(Client, host, configBasic.Token,
            TimeSpan.FromHours(configBasic.RestartPeriodHours), Logger);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        Operations.Sort();

        await _connection.StartAsync(cancellationToken);

        _ticker.Start(cancellationToken);

        await UpdateCommands(null, cancellationToken);
    }

    public void AddOrUpdateAccesses(Dictionary<long, AccessData> toAdd)
    {
        foreach (long id in toAdd.Keys)
        {
            Accesses[id] = toAdd[id];
        }
    }

    public void Update(Telegram.Bot.Types.Update update) => Invoker.FireAndForget(_ => UpdateAsync(update), Logger);

    public virtual Task StopAsync(CancellationToken cancellationToken) => _connection.StopAsync(cancellationToken);

    public AccessData GetAccess(long userId) => Accesses.ContainsKey(userId) ? Accesses[userId] : AccessData.Default;

    private InputFileId? TryGetFileId(string path)
    {
        return _fileCache.TryGetValue(path, out string? fileId) ? InputFile.FromFileId(fileId) : null;
    }

    private void AddFileIfNew(string path, FileBase? file)
    {
        if (_fileCache.ContainsKey(path))
        {
            return;
        }

        if (file is null)
        {
            return;
        }
        _fileCache[path] = file.FileId;
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, KeyboardProvider? keyboardProvider = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        LinkPreviewOptions? linkPreviewOptions = null, int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        keyboardProvider ??= GetDefaultKeyboardProvider(chat);
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.SendText, Logger, data: text);
        return Client.SendMessage(chat.Id, text, parseMode, replyParameters, keyboardProvider.Keyboard,
            linkPreviewOptions, messageThreadId, entities, disableNotification, protectContent, messageEffectId,
            businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(Chat chat, int messageId, string text,
        ParseMode parseMode = ParseMode.None, IEnumerable<MessageEntity>? entities = null,
        LinkPreviewOptions? linkPreviewOptions = null, InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.EditText, Logger, messageId, text);
        return Client.EditMessageText(chat.Id, messageId, text, parseMode, entities, linkPreviewOptions, replyMarkup,
            businessConnectionId, cancellationToken);
    }

    public async Task<Message> EditMessageMediaAsync(Chat chat, int messageId, string path,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        InputFile? photo = TryGetFileId(path);
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

            AddFileIfNew(path, message.Photo.Largest());

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
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.EditMedia, Logger, messageId);
        return Client.EditMessageMedia(chat.Id, messageId, media, replyMarkup, businessConnectionId,
            cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(Chat chat, int messageId, string? caption,
        ParseMode parseMode = ParseMode.None, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.EditText, Logger, messageId);
        return Client.EditMessageCaption(chat.Id, messageId, caption, parseMode, captionEntities,
            showCaptionAboveMedia, replyMarkup, businessConnectionId, cancellationToken);
    }

    public Task DeleteMessageAsync(Chat chat, int messageId, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.Delete, Logger, messageId);
        return Client.DeleteMessage(chat.Id, messageId, cancellationToken);
    }

    public Task<Message> ForwardMessageAsync(Chat chat, ChatId fromChatId, int messageId, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, int? videoStartTimestamp = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.Forward, Logger, data: $"message {messageId} from {fromChatId}");
        return Client.ForwardMessage(chat.Id, fromChatId, messageId, messageThreadId, disableNotification,
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
            InputFile? photo = TryGetFileId(path);
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
                AddFileIfNew(path, messages[i].Photo.Largest());
            }
        }
        else
        {
            Logger.LogError("Wrong MediaGroup size", $"Recieved {messages.Length} after {paths.Count} paths");
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
        DelayIfNeeded(chat, cancellationToken);
        List<IAlbumInputMedia> all = new(media);
        string captions = string.Join(", ", all.OfType<InputMedia>().Select(m => m.Caption).SkipNulls());
        Logging.Update.Log(chat, Logging.Update.Type.SendFiles, Logger, data: captions);

        return Client.SendMediaGroup(chat.Id, all, replyParameters, messageThreadId, disableNotification,
            protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public async Task<Message> SendPhotoAsync(Chat chat, string path, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, bool hasSpoiler = false, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default)
    {
        InputFile? photo = TryGetFileId(path);
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

            AddFileIfNew(path, message.Photo.Largest());

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
        keyboardProvider ??= GetDefaultKeyboardProvider(chat);
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.SendPhoto, Logger, data: caption);

        return Client.SendPhoto(chat.Id, photo, caption, parseMode, replyParameters, keyboardProvider.Keyboard,
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
        InputFile? file = TryGetFileId(path);
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

            AddFileIfNew(path, message.Document);

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
        keyboardProvider ??= GetDefaultKeyboardProvider(chat);
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.SendPhoto, Logger, data: caption);

        return Client.SendDocument(chat.Id, document, caption, parseMode, replyParameters, keyboardProvider.Keyboard,
            thumbnail, messageThreadId, captionEntities, disableContentTypeDetection, disableNotification, protectContent,
            messageEffectId, businessConnectionId, allowPaidBroadcast, cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputFile sticker, ReplyParameters? replyParameters = null,
        KeyboardProvider? keyboardProvider = null, int? messageThreadId = null, string? emoji = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default)
    {
        keyboardProvider ??= GetDefaultKeyboardProvider(chat);
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.SendSticker, Logger);

        return Client.SendSticker(chat.Id, sticker, replyParameters, keyboardProvider.Keyboard, emoji, messageThreadId,
            disableNotification, protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast,
            cancellationToken);
    }

    public Task PinChatMessageAsync(Chat chat, int messageId, bool disableNotification = false,
        string? businessConnectionId = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.Pin, Logger, messageId);
        return Client.PinChatMessage(chat.Id, messageId, disableNotification, businessConnectionId, cancellationToken);
    }

    public Task UnpinChatMessageAsync(Chat chat, int? messageId = null, string? businessConnectionId = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.Unpin, Logger, messageId);
        return Client.UnpinChatMessage(chat.Id, messageId, businessConnectionId, cancellationToken);
    }

    public Task UnpinAllChatMessagesAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.UnpinAll, Logger);
        return Client.UnpinAllChatMessages(chat.Id, cancellationToken);
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
        CancellationToken cancellationToken = default
    )
    {
        DelayIfNeeded(chat, cancellationToken);
        Logging.Update.Log(chat, Logging.Update.Type.SendInvoice, Logger, null, title);
        return Client.SendInvoice(chat.Id, title, description, payload, currency, prices, providerToken, providerData,
            maxTipAmount, suggestedTipAmounts, photoUrl, photoSize, photoWidth, photoHeight, needName, needPhoneNumber,
            needEmail, needShippingAddress, sendPhoneNumberToProvider, sendEmailToProvider, isFlexible,
            replyParameters, replyMarkup, startParameter, messageThreadId, disableNotification, protectContent,
            messageEffectId, allowPaidBroadcast, cancellationToken);
    }

    protected internal Task UpdateCommandsFor(long userId, string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<ICommand> commands = GetMenuCommands();

        return Client.SetMyCommands(commands.Where(c => GetAccess(userId).IsSufficientAgainst(c.AccessRequired))
                                            .Select(ca => ca.BotCommand),
            BotCommandScope.Chat(userId), languageCode, cancellationToken);
    }

    protected virtual async Task UpdateAsync(Message message)
    {
        if (message.From is null)
        {
            throw new Exception("Message update with null From");
        }

        if (message.From.Id == Self.Id)
        {
            return;
        }

        await UpdateAsync(message, message.From);
    }

    protected virtual Task UpdateAsync(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Message is null)
        {
            throw new Exception("CallbackQuery update with null Message");
        }

        if (string.IsNullOrWhiteSpace(callbackQuery.Data))
        {
            throw new Exception("CallbackQuery update with null Data");
        }

        return UpdateAsync(callbackQuery.Message, callbackQuery.From, callbackQuery.Data);
    }

    protected virtual async Task<OperationBasic?> UpdateAsync(Message message, User sender,
        string? callbackQueryData = null)
    {
        if (string.IsNullOrWhiteSpace(callbackQueryData))
        {
            Logging.Update.Log(message.Chat, Logging.Update.Type.ReceiveMessage, Logger, message.MessageId,
                $"{message.Text}{message.Caption}");
        }
        else
        {
            Logging.Update.Log(message.Chat, Logging.Update.Type.ReceiveCallback, Logger, message.MessageId,
                callbackQueryData);
        }

        foreach (OperationBasic operation in Operations)
        {
            if (message.Chat.IsGroup() && !operation.EnabledInGroups)
            {
                continue;
            }

            if ((message.Chat.Type == ChatType.Channel) && !operation.EnabledInChannels)
            {
                continue;
            }

            OperationBasic.ExecutionResult result =
                await operation.TryExecuteAsync(this, message, sender, callbackQueryData);
            switch (result)
            {
                case OperationBasic.ExecutionResult.UnsuitableOperation: continue;
                case OperationBasic.ExecutionResult.AccessInsufficent:
                    await ProcessInsufficientAccess(message, sender, operation);
                    return operation;
                case OperationBasic.ExecutionResult.AccessExpired:
                    await ProcessExpiredAccess(message, sender, operation);
                    return operation;
                case OperationBasic.ExecutionResult.Success: return operation;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        await ProcessUnclearOperation(message, sender);
        return null;
    }

    protected virtual Task UpdateAsync(PreCheckoutQuery _) => Task.CompletedTask;

    protected virtual Task ProcessUnclearOperation(Message message, User _)
    {
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return message.Chat.IsGroup() ? Task.CompletedTask : SendStickerAsync(message.Chat, DontUnderstandSticker, rp);
    }

    protected virtual Task ProcessInsufficientAccess(Message message, User _, OperationBasic __)
    {
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return message.Chat.IsGroup() ? Task.CompletedTask : SendStickerAsync(message.Chat, ForbiddenSticker, rp);
    }

    protected virtual Task ProcessExpiredAccess(Message message, User _, OperationBasic __)
    {
        return ProcessInsufficientAccess(message, _, __);
    }

    protected virtual KeyboardProvider GetDefaultKeyboardProvider(Chat _) => KeyboardProvider.Remove;

    private Task UpdateAsync(Telegram.Bot.Types.Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => UpdateAsync(update.Message.Denull(nameof(update.Message))),
            UpdateType.CallbackQuery => UpdateAsync(update.CallbackQuery.Denull(nameof(update.CallbackQuery))),
            UpdateType.PreCheckoutQuery =>
                UpdateAsync(update.PreCheckoutQuery.Denull(nameof(update.PreCheckoutQuery))),
            _ => Task.CompletedTask
        };
    }

    private async Task UpdateCommands(string? languageCode, CancellationToken cancellationToken)
    {
        await Client.DeleteMyCommands(cancellationToken: cancellationToken);
        await Client.DeleteMyCommands(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await Client.DeleteMyCommands(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        List<ICommand> commands = GetMenuCommands().ToList();
        await Client.SetMyCommands(
            commands.Where(c => AccessData.Default.IsSufficientAgainst(c.AccessRequired))
                    .Select(ca => ca.BotCommand),
            BotCommandScope.AllPrivateChats(), languageCode, cancellationToken);

        foreach (long userId in Accesses.Keys)
        {
            await Client.SetMyCommands(
                commands.Where(c => Accesses[userId].IsSufficientAgainst(c.AccessRequired))
                        .Select(ca => ca.BotCommand),
                BotCommandScope.Chat(userId), languageCode, cancellationToken);
        }
    }

    private void DelayIfNeeded(Chat chat, CancellationToken cancellationToken)
    {
        lock (_delayLocker)
        {
            DateTimeFull now = DateTimeFull.CreateUtcNow();

            TimeSpan? beforeGlobalUpdate = Clock.GetDelayUntil(_lastUpdateGlobal, _sendMessagePeriodGlobal, now);

            DateTimeFull? lastUpdateLocal = _lastUpdates.ContainsKey(chat.Id) ? _lastUpdates[chat.Id] : null;
            TimeSpan period = chat.Type == ChatType.Private ? _sendMessagePeriodPrivate : _sendMessagePeriodGroup;
            TimeSpan? beforeLocalUpdate = Clock.GetDelayUntil(lastUpdateLocal, period, now);

            TimeSpan? maxDelay = TimeSpanExtensions.Max(beforeGlobalUpdate, beforeLocalUpdate);
            if (maxDelay.HasValue)
            {
                Task.Delay(maxDelay.Value, cancellationToken).Wait(cancellationToken);
                now += maxDelay.Value;
            }

            _lastUpdateGlobal = now;
            _lastUpdates[chat.Id] = now;
        }
    }

    private Dictionary<long, AccessData> GetAccesses()
    {
        return ConfigBasic.Accesses.Count > 0
            ? ConfigBasic.Accesses.ToDictionary(p => p.Key, p => new AccessData(p.Value))
            : new Dictionary<long, AccessData>();
    }

    private IEnumerable<ICommand> GetMenuCommands() => Operations.OfType<ICommand>().Where(c => c.ShowInMenu);

    private readonly Dictionary<string, string> _fileCache = new();

    private readonly Dictionary<long, DateTimeFull> _lastUpdates = new();
    private readonly object _delayLocker = new();

    private readonly Ticker _ticker;

    private readonly TimeSpan _sendMessagePeriodPrivate;
    private readonly TimeSpan _sendMessagePeriodGlobal;
    private readonly TimeSpan _sendMessagePeriodGroup;

    private DateTimeFull? _lastUpdateGlobal;

    private readonly IConnection _connection;
}