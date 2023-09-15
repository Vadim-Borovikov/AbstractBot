using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Configs;
using AbstractBot.Contexts;
using AbstractBot.Extensions;
using AbstractBot.Operations;
using AbstractBot.Operations.Commands;
using GryphonUtilities;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class BotBasic
{
    public string Host { get; private set; } = "";

    public readonly TelegramBotClient Client;
    public readonly ConfigBasic Config;
    public readonly TimeManager TimeManager;
    public readonly Logger Logger;
    public readonly JsonSerializerOptionsProvider JsonSerializerOptionsProvider;

    public readonly Dictionary<long, Context> Contexts = new();

    public const int DefaultAccess = 1;

    public User? User;

    protected static readonly ReplyKeyboardRemove NoKeyboard = new();

    protected internal readonly List<OperationBasic> Operations;

    internal readonly string About;

    protected readonly Dictionary<long, int> Accesses;
    protected readonly InputFileId DontUnderstandSticker;
    protected readonly InputFileId ForbiddenSticker;

    protected BotBasic(ConfigBasic config)
    {
        Config = config;

        Client = new TelegramBotClient(Config.Token);

        Operations = new List<OperationBasic>
        {
            new Help(this)
        };

        DontUnderstandSticker = new InputFileId(Config.DontUnderstandStickerFileId);
        ForbiddenSticker = new InputFileId(Config.ForbiddenStickerFileId);

        TimeManager = new TimeManager(Config.SystemTimeZoneId);
        Logger = new Logger(TimeManager);
        _ticker = new Ticker(Logger);

        JsonSerializerOptionsProvider = new JsonSerializerOptionsProvider(TimeManager);

        _sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitPrivate);
        _sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitGlobal);
        _sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / config.UpdatesPerMinuteLimitGroup);
        Accesses = GetAccesses();

        About = Config.Texts.AboutLinesMarkdownV2 is null ? "" : Text.JoinLines(Config.Texts.AboutLinesMarkdownV2);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        Operations.Sort();
        Host = await GetHostAsync();
        string url = $"{Host}/{Config.Token}";
        await Client.SetWebhookAsync(url, cancellationToken: cancellationToken,
            allowedUpdates: Array.Empty<UpdateType>());
        _ticker.Start(cancellationToken);

        await UpdateCommands(cancellationToken);

        User = await Client.GetMeAsync(cancellationToken);

        Invoker.DoPeriodically(ReconnectAsync, TimeSpan.FromHours(Config.RestartPeriodHours), false, Logger,
            cancellationToken);
    }

    public void Update(Update update) => Invoker.FireAndForget(_ => UpdateAsync(update), Logger);

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        return Client.DeleteWebhookAsync(false, cancellationToken);
    }

    public int GetAccess(long userId) => Accesses.ContainsKey(userId) ? Accesses[userId] : DefaultAccess;

    public T? TryGetContext<T>(long key) where T : Context
    {
        return Contexts.TryGetValue(key, out Context? value) ? value as T : null;
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, ParseMode? parseMode = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return SendTextMessageAsync(chat, text, GetDefaultKeyboard(chat), parseMode, messageThreadId, entities,
            disableWebPagePreview, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply,
            cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, IReplyMarkup? replyMarkup,
        ParseMode? parseMode = null, int? messageThreadId = null, IEnumerable<MessageEntity>? entities = null,
        bool? disableWebPagePreview = null, bool? disableNotification = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendText, Logger, data: text);
        return Client.SendTextMessageAsync(chat.Id, text, messageThreadId, parseMode, entities, disableWebPagePreview,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply,
            replyMarkup, cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(Chat chat, int messageId, string text, ParseMode? parseMode = null,
        IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        InlineKeyboardMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.EditText, Logger, messageId, text);
        return Client.EditMessageTextAsync(chat.Id, messageId, text, parseMode, entities,
            disableWebPagePreview, replyMarkup, cancellationToken);
    }

    public Task<Message> EditMessageCaptionAsync(Chat chat, int messageId, string? caption, ParseMode? parseMode = null,
        IEnumerable<MessageEntity>? captionEntities = null, InlineKeyboardMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.EditText, Logger, messageId);
        return Client.EditMessageCaptionAsync(chat.Id, messageId, caption, parseMode, captionEntities, replyMarkup,
            cancellationToken);
    }

    public Task DeleteMessageAsync(Chat chat, int messageId, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Delete, Logger, messageId);
        return Client.DeleteMessageAsync(chat.Id, messageId, cancellationToken);
    }

    public Task<Message> ForwardMessageAsync(Chat chat, ChatId fromChatId, int messageId, int? messageThreadId = null,
        bool? disableNotification = null, bool? protectContent = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Forward, Logger, data: $"message {messageId} from {fromChatId}");
        return Client.ForwardMessageAsync(chat.Id, fromChatId, messageId, messageThreadId, disableNotification,
            protectContent, cancellationToken);
    }

    public async Task<Message[]> SendMediaGroupAsync(Chat chat, IList<string> paths, string? caption = null,
        ParseMode? parseMode = null, int? messageThreadId = null, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        List<FileStream> streams = paths.Select(System.IO.File.OpenRead).ToList();
        List<InputFile> inputFiles = streams.Select(FileStreamExtensions.ToInputFileStream).Cast<InputFile>().ToList();

        Message[] messages = await SendMediaGroupAsync(chat, inputFiles, caption, parseMode, messageThreadId,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);

        foreach (FileStream stream in streams)
        {
            await stream.DisposeAsync();
        }
        Parallel.ForEach(paths, System.IO.File.Delete);

        return messages;
    }

    public Task<Message[]> SendMediaGroupAsync(Chat chat, IList<InputFile> inputFiles, string? caption = null,
        ParseMode? parseMode = null, int? messageThreadId = null, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
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

        return SendMediaGroupAsync(chat, media, messageThreadId, disableNotification, protectContent, replyToMessageId,
            allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message[]> SendMediaGroupAsync(Chat chat, IEnumerable<IAlbumInputMedia> media,
        int? messageThreadId = null, bool? disableNotification = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        List<IAlbumInputMedia> all = new(media);
        string captions = string.Join(", ", all.OfType<InputMedia>().Select(m => m.Caption).RemoveNulls());
        UpdateInfo.Log(chat, UpdateInfo.Type.SendFiles, Logger, data: captions);
        return Client.SendMediaGroupAsync(chat.Id, all, messageThreadId, disableNotification, protectContent,
            replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(Chat chat, InputFile photo, int? messageThreadId = null,
        string? caption = null, ParseMode? parseMode = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool? hasSpoiler = null, bool? disableNotification = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        return SendPhotoAsync(chat, photo, GetDefaultKeyboard(chat), messageThreadId, caption, parseMode,
            captionEntities, hasSpoiler, disableNotification, protectContent, replyToMessageId,
            allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(Chat chat, InputFile photo, IReplyMarkup? replyMarkup,
        int? messageThreadId = null, string? caption = null, ParseMode? parseMode = null,
        IEnumerable<MessageEntity>? captionEntities = null, bool? hasSpoiler = null, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendPhoto, Logger, data: caption);
        return Client.SendPhotoAsync(chat.Id, photo, messageThreadId, caption, parseMode, captionEntities, hasSpoiler,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, replyMarkup,
            cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputFile sticker, int? messageThreadId = null,
        string? emoji = null, bool? disableNotification = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        return SendStickerAsync(chat, sticker, GetDefaultKeyboard(chat), messageThreadId, emoji,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputFile sticker, IReplyMarkup? replyMarkup,
        int? messageThreadId = null, string? emoji = null, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendSticker, Logger);
        return Client.SendStickerAsync(chat.Id, sticker, messageThreadId, emoji, disableNotification, protectContent,
            replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
    }

    public Task PinChatMessageAsync(Chat chat, int messageId, bool? disableNotification = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Pin, Logger, messageId);
        return Client.PinChatMessageAsync(chat.Id, messageId, disableNotification, cancellationToken);
    }

    public Task UnpinChatMessageAsync(Chat chat, int? messageId = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Unpin, Logger, messageId);
        return Client.UnpinChatMessageAsync(chat.Id, messageId, cancellationToken);
    }

    public Task SendInvoiceAsync(Chat chat, string title, string description, string payload, string providerToken,
        string currency, IEnumerable<LabeledPrice> prices, int? messageThreadId = null, int? maxTipAmount = null,
        IEnumerable<int>? suggestedTipAmounts = null, string? startParameter = null, string? providerData = null,
        string? photoUrl = null, int? photoSize = null, int? photoWidth = null, int? photoHeight = null,
        bool? needName = null, bool? needPhoneNumber = null, bool? needEmail = null, bool? needShippingAddress = null,
        bool? sendPhoneNumberToProvider = null, bool? sendEmailToProvider = null, bool? isFlexible = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, InlineKeyboardMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendInvoice, Logger, null, title);
        return Client.SendInvoiceAsync(chat.Id, title, description, payload, providerToken, currency, prices,
            messageThreadId, maxTipAmount, suggestedTipAmounts, startParameter, providerData, photoUrl, photoSize,
            photoWidth, photoHeight, needName, needPhoneNumber, needEmail, needShippingAddress,
            sendPhoneNumberToProvider, sendEmailToProvider, isFlexible, disableNotification, protectContent,
            replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
    }

    protected internal Task UpdateCommandsFor(long userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<ICommand> commands = GetMenuCommands();
        int access = GetAccess(userId);
        return Client.SetMyCommandsAsync(commands.Where(c => AccessHelpers.IsSufficient(access, c.AccessRequired))
                                                 .Select(ca => ca.BotCommand),
            BotCommandScope.Chat(userId), cancellationToken: cancellationToken);
    }

    protected virtual Task UpdateAsync(Message message)
    {
        if (message.From is null)
        {
            throw new Exception("Message update with null From");
        }

        return UpdateAsync(message, message.From);
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

    protected virtual async Task UpdateAsync(Message message, User sender, string? callbackQueryData = null)
    {
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
                await operation.TryExecuteAsync(message, sender, callbackQueryData);
            switch (result)
            {
                case OperationBasic.ExecutionResult.UnsuitableOperation: continue;
                case OperationBasic.ExecutionResult.InsufficentAccess:
                    await ProcessInsufficientAccess(message, sender, operation);
                    return;
                case OperationBasic.ExecutionResult.Success: return;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        await ProcessUnclearOperation(message, sender);
    }

    protected virtual Task UpdateAsync(PreCheckoutQuery _) => Task.CompletedTask;

    protected virtual Task ProcessUnclearOperation(Message message, User _)
    {
        return message.Chat.IsGroup()
            ? Task.CompletedTask
            : SendStickerAsync(message.Chat, DontUnderstandSticker, replyToMessageId: message.MessageId);
    }

    protected virtual Task ProcessInsufficientAccess(Message message, User _, OperationBasic __)
    {
        return message.Chat.IsGroup()
            ? Task.CompletedTask
            : SendStickerAsync(message.Chat, ForbiddenSticker, replyToMessageId: message.MessageId);
    }

    protected virtual IReplyMarkup GetDefaultKeyboard(Chat _) => NoKeyboard;

    private Task UpdateAsync(Update update)
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

    private async Task UpdateCommands(CancellationToken cancellationToken)
    {
        await Client.DeleteMyCommandsAsync(cancellationToken: cancellationToken);
        await Client.DeleteMyCommandsAsync(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await Client.DeleteMyCommandsAsync(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        List<ICommand> commands = GetMenuCommands().ToList();
        await Client.SetMyCommandsAsync(
            commands.Where(c => AccessHelpers.IsSufficient(DefaultAccess, c.AccessRequired))
                    .Select(ca => ca.BotCommand),
            BotCommandScope.AllPrivateChats(), cancellationToken: cancellationToken);

        foreach (long userId in Accesses.Keys)
        {
            await Client.SetMyCommandsAsync(
                commands.Where(c => AccessHelpers.IsSufficient(Accesses[userId], c.AccessRequired))
                        .Select(ca => ca.BotCommand),
                BotCommandScope.Chat(userId), cancellationToken: cancellationToken);
        }
    }

    private void DelayIfNeeded(Chat chat, CancellationToken cancellationToken)
    {
        lock (_delayLocker)
        {
            DateTimeFull now = DateTimeFull.CreateUtcNow();

            TimeSpan? beforeGlobalUpdate = TimeManager.GetDelayUntil(_lastUpdateGlobal, _sendMessagePeriodGlobal, now);

            DateTimeFull? lastUpdateLocal = _lastUpdates.ContainsKey(chat.Id) ? _lastUpdates[chat.Id] : null;
            TimeSpan period = chat.Type == ChatType.Private ? _sendMessagePeriodPrivate : _sendMessagePeriodGroup;
            TimeSpan? beforeLocalUpdate = TimeManager.GetDelayUntil(lastUpdateLocal, period, now);

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

    private Task<string> GetHostAsync()
    {
        return string.IsNullOrWhiteSpace(Config.Host)
            ? Ngrok.Manager.GetHostAsync(JsonSerializerOptionsProvider.SnakeCaseOptions)
            : Task.FromResult(Config.Host);
    }

    private Dictionary<long, int> GetAccesses()
    {
        if (Config.Accesses is not null && (Config.Accesses.Count > 0))
        {
            return Config.Accesses;
        }

        if (!string.IsNullOrWhiteSpace(Config.AccessesJson))
        {
            Dictionary<long, int>? deserialized =
                JsonSerializer.Deserialize<Dictionary<long, int>>(Config.AccessesJson,
                    JsonSerializerOptionsProvider.PascalCaseOptions);
            if (deserialized is not null)
            {
                return deserialized;
            }
        }

        return new Dictionary<long, int>();
    }

    private IEnumerable<ICommand> GetMenuCommands() => Operations.OfType<ICommand>().Where(c => !c.HideFromMenu);

    private async Task ReconnectAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogTimedMessage("Reconnecting to Telegram...");

        await Client.DeleteWebhookAsync(false, cancellationToken);

        string url = $"{Host}/{Config.Token}";
        await Client.SetWebhookAsync(url, allowedUpdates: Array.Empty<UpdateType>(),
            cancellationToken: cancellationToken);

        Logger.LogTimedMessage("...connected.");
    }

    private readonly Dictionary<long, DateTimeFull> _lastUpdates = new();
    private readonly object _delayLocker = new();

    private readonly Ticker _ticker;

    private readonly TimeSpan _sendMessagePeriodPrivate;
    private readonly TimeSpan _sendMessagePeriodGlobal;
    private readonly TimeSpan _sendMessagePeriodGroup;

    private DateTimeFull? _lastUpdateGlobal;
}