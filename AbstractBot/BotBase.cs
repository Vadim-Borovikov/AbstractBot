using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
public abstract class BotBase<TBot, TConfig>
    where TBot: BotBase<TBot, TConfig>
    where TConfig: Config
{
    public enum AccessType
    {
        SuperAdmin,
        Admins,
        Users
    }

    public readonly TelegramBotClient Client;
    public readonly TConfig Config;
    public readonly TimeManager TimeManager;

    protected readonly List<CommandBase<TBot, TConfig>> Commands;
    protected readonly InputOnlineFile DontUnderstandSticker;
    protected readonly InputOnlineFile ForbiddenSticker;

    protected BotBase(TConfig config)
    {
        Config = config;

        Client = new TelegramBotClient(Config.Token);

        Commands = new List<CommandBase<TBot, TConfig>>();

        DontUnderstandSticker = new InputOnlineFile(Config.DontUnderstandStickerFileId);
        ForbiddenSticker = new InputOnlineFile(Config.ForbiddenStickerFileId);

        TimeManager = new TimeManager(Config.SystemTimeZoneId);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await Config.UpdateHostIfNeededAsync(Utils.GetNgrokHostAsync());
        await Client.SetWebhookAsync(Config.Url, cancellationToken: cancellationToken,
            allowedUpdates: Array.Empty<UpdateType>());
        TickManager.Start(cancellationToken);
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        return Client.DeleteWebhookAsync(false, cancellationToken);
    }

    public void Update(Update update) => Utils.FireAndForget(_ => UpdateAsync(update));

    public Task<User> GetUserAsync() => Client.GetMeAsync();

    public string GetDescriptionFor(long userId)
    {
        AccessType access = GetMaximumAccessFor(userId);
        return GetDescription(access);
    }

    public string GetCommandsDescriptionFor(long userId)
    {
        AccessType access = GetMaximumAccessFor(userId);
        return GetCommandsDescription(access);
    }

    public bool IsAdmin(long userId) => Config.AdminIds.Contains(userId);
    public bool IsSuperAdmin(long userId) => Config.SuperAdminId == userId;

    public bool IsAccessSuffice(long userId, AccessType against)
    {
        switch (against)
        {
            case AccessType.SuperAdmin when IsSuperAdmin(userId):
            case AccessType.Admins when IsAdmin(userId) || IsSuperAdmin(userId):
            case AccessType.Users:
                return true;
            default: return false;
        }
    }

    public Task<Message> SendTextMessageAsync(Chat chat, string text, ParseMode? parseMode = null,
        IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
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
        bool? allowSendingWithoutReply = null, IReplyMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default)
    {
        DelayIfNeeded(chat, cancellationToken);
        UpdateInfo.Log(chat, UpdateInfo.Type.SendPhoto, data: caption);
        return Client.SendPhotoAsync(chat.Id, photo, caption, parseMode, captionEntities, disableNotification,
            protectContent, replyToMessageId, allowSendingWithoutReply, replyMarkup, cancellationToken);
    }

    public Task<Message> SendStickerAsync(Chat chat, InputOnlineFile sticker, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        IReplyMarkup? replyMarkup = null, CancellationToken cancellationToken = default)
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

    public Task<Message> FinalizeStatusMessageAsync(Message message, string postfix = "")
    {
        string text = $"_{message.Text}_ Готово\\.{postfix}";
        return EditMessageTextAsync(message.Chat, message.MessageId, text, ParseMode.MarkdownV2);
    }

    protected virtual Task UpdateAsync(Message message, bool fromChat, CommandBase<TBot, TConfig>? command = null,
        string? payload = null)
    {
        return message.Type switch
        {
            MessageType.Text              => ProcessTextMessageAsync(message, fromChat, command, payload),
            MessageType.SuccessfulPayment => ProcessSuccessfulPaymentMessageAsync(message, fromChat),
            _                             => SendStickerAsync(message.Chat, DontUnderstandSticker)
        };
    }

    protected virtual Task ProcessTextMessageAsync(Message textMessage, bool fromChat,
        CommandBase<TBot, TConfig>? command = null, string? payload = null)
    {
        if (command is null)
        {
            return SendStickerAsync(textMessage.Chat, DontUnderstandSticker);
        }

        User user = textMessage.From.GetValue(nameof(textMessage.From));
        bool shouldExecute = IsAccessSuffice(user.Id, command.Access);
        return shouldExecute ? command.ExecuteAsync(textMessage, fromChat, payload)
                             : SendStickerAsync(textMessage.Chat, ForbiddenSticker);
    }

    protected virtual Task ProcessCallbackAsync(CallbackQuery callback) => Task.CompletedTask;

    protected virtual Task ProcessPreCheckoutAsync(PreCheckoutQuery preCheckout) => Task.CompletedTask;

    protected virtual Task ProcessSuccessfulPaymentMessageAsync(Message successfulPaymentMessage, bool fromChat)
    {
        return Task.CompletedTask;
    }

    private void DelayIfNeeded(Chat chat, CancellationToken cancellationToken)
    {
        lock (_delayLocker)
        {
            DateTime now = TimeManager.Now();

            TimeSpan? beforeGlobalUpdate =
                TimeManager.GetDelayUntil(_lastUpdateGlobal, Config.SendMessagePeriodGlobal, now);

            DateTime? lastUpdateLocal = _lastUpdates.GetValueOrDefault(chat.Id);
            TimeSpan period = chat.Type == ChatType.Private
                ? Config.SendMessagePeriodPrivate
                : Config.SendMessagePeriodGroup;
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

    private Task UpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => UpdateAsync(update.Message.GetValue(nameof(update.Message))),
            UpdateType.CallbackQuery =>
                ProcessCallbackAsync(update.CallbackQuery.GetValue(nameof(update.CallbackQuery))),
            UpdateType.PreCheckoutQuery =>
                ProcessPreCheckoutAsync(update.PreCheckoutQuery.GetValue(nameof(update.PreCheckoutQuery))),
            _ => Task.CompletedTask
        };
    }

    private async Task UpdateAsync(Message message)
    {
        User user = message.From.GetValue(nameof(message.From));
        bool fromChat = message.Chat.Id != user.Id;
        string? botName = null;
        if (fromChat)
        {
            User bot = await GetUserAsync();
            botName = bot.Username;
        }

        foreach (CommandBase<TBot, TConfig> command in Commands)
        {
            if (command.IsInvokingBy(message.Text ?? "", out string? payload, fromChat, botName))
            {
                await UpdateAsync(message, fromChat, command, payload);
                return;
            }
        }

        await UpdateAsync(message, fromChat);
    }

    private AccessType GetMaximumAccessFor(long userId)
    {
        return IsSuperAdmin(userId)
            ? AccessType.SuperAdmin
            : IsAdmin(userId) ? AccessType.Admins : AccessType.Users;
    }

    private string GetDescription(AccessType access)
    {
        string commandsDescription = GetCommandsDescription(access);

        if (string.IsNullOrWhiteSpace(Config.About))
        {
            return commandsDescription;
        }

        if (string.IsNullOrWhiteSpace(commandsDescription))
        {
            return Config.About;
        }

        return $"{Config.About}{Environment.NewLine}{Environment.NewLine}{commandsDescription}";
    }

    private string GetCommandsDescription(AccessType access)
    {
        StringBuilder builder = new();
        List<CommandBase<TBot, TConfig>> userCommands = Commands.Where(c => c.Access == AccessType.Users).ToList();
        if (access != AccessType.Users)
        {
            List<CommandBase<TBot, TConfig>> adminCommands = Commands.Where(c => c.Access == AccessType.Admins).ToList();
            if (access == AccessType.SuperAdmin)
            {
                List<CommandBase<TBot, TConfig>> superAdminCommands =
                    Commands.Where(c => c.Access == AccessType.SuperAdmin).ToList();
                if (superAdminCommands.Any())
                {
                    builder.AppendLine(superAdminCommands.Count > 1 ? "Команды суперадмина:" : "Команда суперадмина:");
                    foreach (CommandBase<TBot, TConfig> command in superAdminCommands)
                    {
                        builder.AppendLine($"/{command.Name} – {command.Description}");
                    }
                    if (adminCommands.Any() || userCommands.Any())
                    {
                        builder.AppendLine();
                    }
                }
            }

            if (adminCommands.Any())
            {
                builder.AppendLine(adminCommands.Count > 1 ? "Админские команды:" : "Админская команда:");
                foreach (CommandBase<TBot, TConfig> command in adminCommands)
                {
                    builder.AppendLine($"/{command.Name} – {command.Description}");
                }
                if (userCommands.Any())
                {
                    builder.AppendLine();
                }
            }
        }

        if (userCommands.Any())
        {
            builder.AppendLine(userCommands.Count > 1 ? "Команды:" : "Команда:");
            foreach (CommandBase<TBot, TConfig> command in userCommands)
            {
                builder.AppendLine($"/{command.Name} – {command.Description}");
            }
        }

        if (!string.IsNullOrWhiteSpace(Config.ExtraCommands))
        {
            builder.AppendLine(Config.ExtraCommands);
        }

        return builder.ToString();
    }

    private readonly Dictionary<long, DateTime> _lastUpdates = new();
    private DateTime? _lastUpdateGlobal;
    private readonly object _delayLocker = new();
}