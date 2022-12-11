using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Commands;
using AbstractBot.Operations;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot;

[PublicAPI]
public abstract class BotBase
{
    public string Host { get; private set; } = "";

    public readonly TelegramBotClient Client;
    public readonly Config ConfigBase;
    public readonly TimeManager TimeManager;
    public readonly JsonSerializerOptionsProvider JsonSerializerOptionsProvider;

    public User? User;

    protected internal readonly List<Operation> Operations;

    internal readonly string About;
    internal readonly string? StartPostfix;
    internal readonly string? HelpPrefix;

    protected readonly List<long> AdminIds;
    protected readonly InputOnlineFile DontUnderstandSticker;
    protected readonly InputOnlineFile ForbiddenSticker;

    protected BotBase(Config config)
    {
        ConfigBase = config;

        Client = new TelegramBotClient(ConfigBase.Token);

        Operations = new List<Operation>
        {
            new StartCommand(this),
            new HelpCommand(this)
        };

        DontUnderstandSticker = new InputOnlineFile(ConfigBase.DontUnderstandStickerFileId);
        ForbiddenSticker = new InputOnlineFile(ConfigBase.ForbiddenStickerFileId);

        TimeManager = new TimeManager(ConfigBase.SystemTimeZoneId);

        JsonSerializerOptionsProvider = new JsonSerializerOptionsProvider(TimeManager);

        _sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitPrivate);
        _sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitGlobal);
        _sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / config.UpdatesPerMinuteLimitGroup);
        AdminIds = GetAdminIds();

        About = string.Join(Environment.NewLine, ConfigBase.About);
        StartPostfix =
            ConfigBase.StartPostfix is null ? null : string.Join(Environment.NewLine, ConfigBase.StartPostfix);
        HelpPrefix = ConfigBase.HelpPrefix is null ? null : string.Join(Environment.NewLine, ConfigBase.HelpPrefix);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        Operations.Sort();
        Host = await GetHostAsync();
        string url = $"{Host}/{ConfigBase.Token}";
        await Client.SetWebhookAsync(url, cancellationToken: cancellationToken,
            allowedUpdates: Array.Empty<UpdateType>());
        TickManager.Start(cancellationToken);

        await UpdateCommands(cancellationToken);

        User = await Client.GetMeAsync(cancellationToken);
    }

    public void Update(Update update) => Utils.FireAndForget(_ => UpdateAsync(update));

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        return Client.DeleteWebhookAsync(false, cancellationToken);
    }

    public Operation.Access GetMaximumAccessFor(long userId)
    {
        return IsSuperAdmin(userId)
            ? Operation.Access.SuperAdmin
            : IsAdmin(userId) ? Operation.Access.Admin : Operation.Access.User;
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, ParseMode? parseMode = null,
        IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return SendTextMessageAsync(chat, text, GetDefaultKeyboard(chat), parseMode, entities, disableWebPagePreview,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, IReplyMarkup? replyMarkup,
        ParseMode? parseMode = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendText, data: text);
        return Client.SendTextMessageAsync(chat.Id, text, parseMode, entities, disableWebPagePreview,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, replyMarkup,
            cancellationToken);
    }

    public Task<Message> EditMessageTextAsync(Chat chat, int messageId, string text, ParseMode? parseMode = null,
        IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        InlineKeyboardMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.EditText, messageId, text);
        return Client.EditMessageTextAsync(chat.Id, messageId, text, parseMode, entities,
            disableWebPagePreview, replyMarkup, cancellationToken);
    }

    public Task DeleteMessageAsync(Chat chat, int messageId, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Delete, messageId);
        return Client.DeleteMessageAsync(chat.Id, messageId, cancellationToken);
    }

    public Task<Message> ForwardMessageAsync(Chat chat, ChatId fromChatId, int messageId,
        bool? disableNotification = null, bool? protectContent = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Forward, data: $"message {messageId} from {fromChatId}");
        return Client.ForwardMessageAsync(chat.Id, fromChatId, messageId, disableNotification, protectContent,
            cancellationToken);
    }

    public Task<Message> SendPhotoAsync(Chat chat, InputOnlineFile photo, string? caption = null,
        ParseMode? parseMode = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return SendPhotoAsync(chat, photo, GetDefaultKeyboard(chat), caption, parseMode, captionEntities,
            disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message> SendPhotoAsync(Chat chat, InputOnlineFile photo, IReplyMarkup? replyMarkup,
        string? caption = null, ParseMode? parseMode = null, IEnumerable<MessageEntity>? captionEntities = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendPhoto, data: caption);
        return Client.SendPhotoAsync(chat.Id, photo, caption, parseMode, captionEntities, disableNotification,
            protectContent, replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputOnlineFile sticker, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        return SendStickerAsync(chat, sticker, GetDefaultKeyboard(chat), disableNotification, protectContent,
            replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputOnlineFile sticker, IReplyMarkup? replyMarkup,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendSticker);
        return Client.SendStickerAsync(chat.Id, sticker, disableNotification, protectContent, replyToMessageId,
            allowSendingWithoutReply, replyMarkup, cancellationToken);
    }

    public Task PinChatMessageAsync(Chat chat, int messageId, bool? disableNotification = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Pin, messageId);
        return Client.PinChatMessageAsync(chat.Id, messageId, disableNotification, cancellationToken);
    }

    public Task UnpinChatMessageAsync(Chat chat, int? messageId = null, CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.Unpin, messageId);
        return Client.UnpinChatMessageAsync(chat.Id, messageId, cancellationToken);
    }

    public Task SendInvoiceAsync(Chat chat, string title, string description, string payload,
        string providerToken, string currency, IEnumerable<LabeledPrice> prices, int? maxTipAmount = null,
        IEnumerable<int>? suggestedTipAmounts = null, string? startParameter = null, string? providerData = null,
        string? photoUrl = null, int? photoSize = null, int? photoWidth = null, int? photoHeight = null,
        bool? needName = null, bool? needPhoneNumber = null, bool? needEmail = null, bool? needShippingAddress = null,
        bool? sendPhoneNumberToProvider = null, bool? sendEmailToProvider = null, bool? isFlexible = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, InlineKeyboardMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendInvoice, null, title);
        return Client.SendInvoiceAsync(chat.Id, title, description, payload, providerToken, currency, prices,
            maxTipAmount, suggestedTipAmounts, startParameter, providerData, photoUrl, photoSize, photoWidth,
            photoHeight, needName, needPhoneNumber, needEmail, needShippingAddress, sendPhoneNumberToProvider,
            sendEmailToProvider, isFlexible, disableNotification, protectContent, replyToMessageId,
            allowSendingWithoutReply, replyMarkup, cancellationToken);
    }

    protected internal virtual Task OnStartCommand(StartCommand start, Message message, long senderId, string payload)
    {
        return start.Greet(message.Chat);
    }

    protected internal Task UpdateCommandsFor(long chatId, CancellationToken cancellationToken = default)
    {
        return UpdateCommandsFor(Operations.OfType<CommandOperation>(), chatId, cancellationToken);
    }

    protected virtual async Task UpdateAsync(Message message)
    {
        long senderId = GetSenderId(message);
        foreach (Operation operation in Operations)
        {
            Operation.ExecutionResult result = await operation.TryExecuteAsync(message, senderId);
            switch (result)
            {
                case Operation.ExecutionResult.UnsuitableOperation: continue;
                case Operation.ExecutionResult.InsufficentAccess:
                    await ProcessInsufficientAccess(message, senderId, operation);
                    return;
                case Operation.ExecutionResult.Success: return;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
        await ProcessUnclearOperation(message, senderId);
    }

    protected virtual Task UpdateAsync(CallbackQuery _) => Task.CompletedTask;

    protected virtual Task UpdateAsync(PreCheckoutQuery _) => Task.CompletedTask;

    protected virtual Task ProcessUnclearOperation(Message message, long senderId)
    {
        return SendStickerAsync(message.Chat, DontUnderstandSticker, replyToMessageId: message.MessageId);
    }

    protected virtual Task ProcessInsufficientAccess(Message message, long senderId, Operation _)
    {
        return SendStickerAsync(message.Chat, ForbiddenSticker, replyToMessageId: message.MessageId);
    }

    protected virtual IReplyMarkup GetDefaultKeyboard(Chat _) => Utils.NoKeyboard;

    private bool IsAdmin(long userId) => AdminIds.Contains(userId);

    private bool IsSuperAdmin(long userId) => ConfigBase.SuperAdminId == userId;

    private Task UpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message       => UpdateAsync(update.Message.GetValue(nameof(update.Message))),
            UpdateType.CallbackQuery => UpdateAsync(update.CallbackQuery.GetValue(nameof(update.CallbackQuery))),
            UpdateType.PreCheckoutQuery =>
                UpdateAsync(update.PreCheckoutQuery.GetValue(nameof(update.PreCheckoutQuery))),
            _ => Task.CompletedTask
        };
    }

    private async Task UpdateCommands(CancellationToken cancellationToken)
    {
        await Client.DeleteMyCommandsAsync(cancellationToken: cancellationToken);
        await Client.DeleteMyCommandsAsync(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await Client.DeleteMyCommandsAsync(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        List<CommandOperation> commands = Operations.OfType<CommandOperation>().ToList();
        await Client.SetMyCommandsAsync(
            commands.Where(c => c.AccessLevel == Operation.Access.User).Select(ca => ca.Command),
            BotCommandScope.AllPrivateChats(), cancellationToken: cancellationToken);

        foreach (long adminId in AdminIds)
        {
            await Client.SetMyCommandsAsync(
                commands.Where(c => c.AccessLevel <= Operation.Access.Admin).Select(ca => ca.Command),
                BotCommandScope.Chat(adminId), cancellationToken: cancellationToken);
        }

        if (ConfigBase.SuperAdminId.HasValue)
        {
            await Client.SetMyCommandsAsync(commands.Select(ca => ca.Command),
                BotCommandScope.Chat(ConfigBase.SuperAdminId.Value), cancellationToken: cancellationToken);
        }
    }

    private Task UpdateCommandsFor(IEnumerable<CommandOperation> commands, long chatId,
        CancellationToken cancellationToken)
    {
        Operation.Access access = GetMaximumAccessFor(chatId);
        return Client.SetMyCommandsAsync(commands.Where(c => c.AccessLevel <= access).Select(ca => ca.Command),
            BotCommandScope.Chat(chatId), cancellationToken: cancellationToken);
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

            TimeSpan? maxDelay = Utils.Max(beforeGlobalUpdate, beforeLocalUpdate);
            if (maxDelay.HasValue)
            {
                Task.Delay(maxDelay.Value, cancellationToken).Wait(cancellationToken);
                now += maxDelay.Value;
            }

            _lastUpdateGlobal = now;
            _lastUpdates[chat.Id] = now;
        }
    }

    private static long GetSenderId(Message message)
    {
        if (message.SenderChat is not null)
        {
            return message.SenderChat.Id;
        }
        User user = message.From.GetValue(nameof(message.From));
        return user.Id;
    }

    private Task<string> GetHostAsync()
    {
        return string.IsNullOrWhiteSpace(ConfigBase.Host)
            ? Utils.GetNgrokHostAsync(JsonSerializerOptionsProvider.SnakeCaseOptions)
            : Task.FromResult(ConfigBase.Host);
    }

    private List<long> GetAdminIds()
    {
        if (ConfigBase.AdminIds is not null && (ConfigBase.AdminIds.Count != 0))
        {
            return ConfigBase.AdminIds;
        }

        if (!string.IsNullOrWhiteSpace(ConfigBase.AdminIdsJson))
        {
            List<long>? deserialized = JsonSerializer.Deserialize<List<long>>(ConfigBase.AdminIdsJson,
                JsonSerializerOptionsProvider.PascalCaseOptions);
            if (deserialized is not null)
            {
                return deserialized;
            }
        }

        return new List<long>();
    }

    private readonly Dictionary<long, DateTimeFull> _lastUpdates = new();
    private readonly object _delayLocker = new();

    private readonly TimeSpan _sendMessagePeriodPrivate;
    private readonly TimeSpan _sendMessagePeriodGlobal;
    private readonly TimeSpan _sendMessagePeriodGroup;

    private DateTimeFull? _lastUpdateGlobal;
}