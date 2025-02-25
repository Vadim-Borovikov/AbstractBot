using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Models;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface IUpdateSender
{
    Task<Message> SendTextMessageAsync(Chat chat, string text, KeyboardProvider? keyboardProvider = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        LinkPreviewOptions? linkPreviewOptions = null, int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    Task<Message> EditMessageTextAsync(Chat chat, int messageId, string text, ParseMode parseMode = ParseMode.None,
        IEnumerable<MessageEntity>? entities = null, LinkPreviewOptions? linkPreviewOptions = null,
        InlineKeyboardMarkup? replyMarkup = null, string? businessConnectionId = null,
        CancellationToken cancellationToken = default);

    Task<Message> EditMessageMediaAsync(Chat chat, int messageId, string path,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default);

    Task<Message> EditMessageMediaAsync(Chat chat, int messageId, InputFile file,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default);

    Task<Message> EditMessageMediaAsync(Chat chat, int messageId, InputMedia media,
        InlineKeyboardMarkup? replyMarkup = default, string? businessConnectionId = default,
        CancellationToken cancellationToken = default);

    Task<Message> EditMessageCaptionAsync(Chat chat, int messageId, string? caption,
        ParseMode parseMode = ParseMode.None, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, InlineKeyboardMarkup? replyMarkup = null,
        string? businessConnectionId = null, CancellationToken cancellationToken = default);

    Task DeleteMessageAsync(Chat chat, int messageId, CancellationToken cancellationToken = default);

    Task<Message> ForwardMessageAsync(Chat chat, ChatId fromChatId, int messageId, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, int? videoStartTimestamp = null,
        CancellationToken cancellationToken = default);

    Task<Message[]> SendMediaGroupAsync(Chat chat, IList<string> paths, string? caption = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    Task<Message[]> SendMediaGroupAsync(Chat chat, IList<InputFile> inputFiles, string? caption = null,
        ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null, int? messageThreadId = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    Task<Message[]> SendMediaGroupAsync(Chat chat, IEnumerable<IAlbumInputMedia> media,
        ReplyParameters? replyParameters = null, int? messageThreadId = null, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default);

    Task<Message> SendPhotoAsync(Chat chat, string path, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, bool hasSpoiler = false, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default);

    Task<Message> SendPhotoAsync(Chat chat, InputFile photo, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool showCaptionAboveMedia = false, bool hasSpoiler = false, bool disableNotification = false,
        bool protectContent = false, string? messageEffectId = null, string? businessConnectionId = null,
        bool allowPaidBroadcast = false, CancellationToken cancellationToken = default);

    Task<Message> SendDocumentAsync(Chat chat, string path, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        InputFile? thumbnail = null, int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool disableContentTypeDetection = false, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    Task<Message> SendDocumentAsync(Chat chat, InputFile document, KeyboardProvider? keyboardProvider = null,
        string? caption = null, ParseMode parseMode = ParseMode.None, ReplyParameters? replyParameters = null,
        InputFile? thumbnail = null, int? messageThreadId = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool disableContentTypeDetection = false, bool disableNotification = false, bool protectContent = false,
        string? messageEffectId = null, string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    Task<Message> SendStickerAsync(Chat chat, InputFile sticker, ReplyParameters? replyParameters = null,
        KeyboardProvider? keyboardProvider = null, int? messageThreadId = null, string? emoji = null,
        bool disableNotification = false, bool protectContent = false, string? messageEffectId = null,
        string? businessConnectionId = null, bool allowPaidBroadcast = false,
        CancellationToken cancellationToken = default);

    Task PinChatMessageAsync(Chat chat, int messageId, bool disableNotification = false,
        string? businessConnectionId = null, CancellationToken cancellationToken = default);

    Task UnpinChatMessageAsync(Chat chat, int? messageId = null, string? businessConnectionId = null,
        CancellationToken cancellationToken = default);

    Task UnpinAllChatMessagesAsync(Chat chat, CancellationToken cancellationToken = default);

    Task SendInvoiceAsync(Chat chat, string title, string description, string payload, string currency,
        IEnumerable<LabeledPrice> prices, string? providerToken = default, string? providerData = default,
        int? maxTipAmount = default, IEnumerable<int>? suggestedTipAmounts = default, string? photoUrl = default,
        int? photoSize = default, int? photoWidth = default, int? photoHeight = default, bool needName = default,
        bool needPhoneNumber = default, bool needEmail = default, bool needShippingAddress = default,
        bool sendPhoneNumberToProvider = default, bool sendEmailToProvider = default, bool isFlexible = default,
        ReplyParameters? replyParameters = default, InlineKeyboardMarkup? replyMarkup = default,
        string? startParameter = default, int? messageThreadId = default, bool disableNotification = default,
        bool protectContent = default, string? messageEffectId = default, bool allowPaidBroadcast = default,
        CancellationToken cancellationToken = default);

    KeyboardProvider GetDefaultKeyboardProvider(Chat chat);
}