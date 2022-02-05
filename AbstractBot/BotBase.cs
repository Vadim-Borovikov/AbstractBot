using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Ngrok;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;

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

        Client = new TelegramBotClient(Config.Token ?? throw new NullReferenceException(nameof(Config.Token)));

        Commands = new List<CommandBase<TBot, TConfig>>();

        DontUnderstandSticker =
            new InputOnlineFile(Config.DontUnderstandStickerFileId
                                ?? throw new NullReferenceException(nameof(Config.DontUnderstandStickerFileId)));
        ForbiddenSticker =
            new InputOnlineFile(Config.ForbiddenStickerFileId
                                ?? throw new NullReferenceException(nameof(Config.ForbiddenStickerFileId)));

        TimeManager = new TimeManager(Config.SystemTimeZoneId);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Config.Host))
        {
            string? ngrokHost = await GetNgrokHost();
            if (string.IsNullOrWhiteSpace(ngrokHost))
            {
                throw new NullReferenceException(nameof(ngrokHost));
            }
            Config.Host = ngrokHost;
        }
        await Client.SetWebhookAsync(Config.Url, cancellationToken: cancellationToken,
            allowedUpdates: Array.Empty<UpdateType>());
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        return Client.DeleteWebhookAsync(false, cancellationToken);
    }

    public Task UpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message          => UpdateAsync(update.Message
                                                       ?? throw new NullReferenceException(nameof(update.Message))),
            UpdateType.CallbackQuery    => ProcessCallbackAsync(update.CallbackQuery
                                                                ?? throw new NullReferenceException(nameof(update.CallbackQuery))),
            UpdateType.PreCheckoutQuery => ProcessPreCheckoutAsync(update.PreCheckoutQuery
                                                                   ?? throw new NullReferenceException(nameof(update.PreCheckoutQuery))),
            _                           => Task.CompletedTask
        };
    }

    public Task<User> GetUserAsync() => Client.GetMeAsync();

    public string GetDescriptionFor(long userId)
    {
        AccessType access = GetMaximumAccessFor(userId);
        return GetDescription(access);
    }

    public string GetAbout() => Config.About is null ? "" : string.Join(Environment.NewLine, Config.About);

    public string GetCommandsDescriptionFor(long userId)
    {
        AccessType access = GetMaximumAccessFor(userId);
        return GetCommandsDescription(access);
    }

    public bool IsAdmin(long userId) => Config.AdminIds is not null && Config.AdminIds.Contains(userId);
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

    protected virtual Task UpdateAsync(Message message, bool fromChat, CommandBase<TBot, TConfig>? command = null,
        string? payload = null)
    {
        return message.Type switch
        {
            MessageType.Text              => ProcessTextMessageAsync(message, fromChat, command, payload),
            MessageType.SuccessfulPayment => ProcessSuccessfulPaymentMessageAsync(message, fromChat),
            _                             => Client.SendStickerAsync(message.Chat.Id, DontUnderstandSticker)
        };
    }

    protected virtual Task ProcessTextMessageAsync(Message textMessage, bool fromChat,
        CommandBase<TBot, TConfig>? command = null, string? payload = null)
    {
        if (command is null)
        {
            return Client.SendStickerAsync(textMessage.Chat.Id, DontUnderstandSticker);
        }

        long userId = textMessage.From?.Id ?? throw new NullReferenceException(nameof(textMessage.From));
        bool shouldExecute = IsAccessSuffice(userId, command.Access);
        return shouldExecute
            ? command.ExecuteAsync(textMessage, fromChat, payload)
            : Client.SendStickerAsync(textMessage.Chat.Id, ForbiddenSticker);
    }

    protected virtual Task ProcessCallbackAsync(CallbackQuery callback) => Task.CompletedTask;

    protected virtual Task ProcessPreCheckoutAsync(PreCheckoutQuery preCheckout) => Task.CompletedTask;

    protected virtual Task ProcessSuccessfulPaymentMessageAsync(Message successfulPaymentMessage, bool fromChat)
    {
        return Task.CompletedTask;
    }

    private async Task<string?> GetNgrokHost()
    {
        ListTunnelsResult? listTunnels = await Provider.ListTunnels();
        return listTunnels?.Tunnels?.Where(t => t.Proto is DesiredNgrokProto).SingleOrDefault()?.PublicUrl;
    }

    private async Task UpdateAsync(Message message)
    {
        long userId = message.From?.Id ?? throw new NullReferenceException(nameof(message.From));
        bool fromChat = message.Chat.Id != userId;
        string? botName = null;
        if (fromChat)
        {
            User user = await GetUserAsync();
            botName = user.Username;
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
        string about = GetAbout();
        string commandsDescription = GetCommandsDescription(access);

        if (string.IsNullOrWhiteSpace(about))
        {
            return commandsDescription;
        }

        if (string.IsNullOrWhiteSpace(commandsDescription))
        {
            return about;
        }

        return $"{about}{Environment.NewLine}{Environment.NewLine}{commandsDescription}";
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

        if (Config.ExtraCommands?.Count > 0)
        {
            foreach (string line in Config.ExtraCommands)
            {
                builder.AppendLine(line);
            }
        }

        return builder.ToString();
    }

    private const string DesiredNgrokProto = "https";
}