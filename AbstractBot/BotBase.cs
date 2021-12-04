using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
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

        protected BotBase(TConfig config)
        {
            Config = config;

            Client = new TelegramBotClient(Config.Token);

            Commands = new List<CommandBase<TBot, TConfig>>();

            DontUnderstandSticker = new InputOnlineFile(Config.DontUnderstandStickerFileId);
            ForbiddenSticker = new InputOnlineFile(Config.ForbiddenStickerFileId);

            TimeManager = new TimeManager(Config.SystemTimeZoneId);
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Client.SetWebhookAsync(Config.Url, cancellationToken: cancellationToken,
                allowedUpdates: new List<UpdateType>());
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Client.DeleteWebhookAsync(false, cancellationToken);
        }

        public Task UpdateAsync(Update update)
        {
            return update?.Type == UpdateType.Message ? UpdateAsync(update.Message) : Task.CompletedTask;
        }

        public Task<User> GetUserAsunc() => Client.GetMeAsync();

        public string GetDescriptionFor(long userId)
        {
            AccessType access = GetMaximumAccessFor(userId);
            return GetDescription(access);
        }

        public string GetAbout() => string.Join(Environment.NewLine, Config.About);

        public string GetCommandsDescriptionFor(long userId)
        {
            AccessType access = GetMaximumAccessFor(userId);
            return GetCommandsDescription(access);
        }

        public bool IsAdmin(long userId) => (Config.AdminIds != null) && Config.AdminIds.Contains(userId);
        public bool IsSuperAdmin(long userId) => Config.SuperAdminId == userId;

        private AccessType GetMaximumAccessFor(long userId)
        {
            return IsSuperAdmin(userId)
                ? AccessType.SuperAdmin
                : (IsAdmin(userId) ? AccessType.Admins : AccessType.Users);
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
            var builder = new StringBuilder();
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

        protected virtual Task UpdateAsync(Message message, bool fromChat, CommandBase<TBot, TConfig> command = null,
            string payload = null)
        {
            if (message.Type != MessageType.Text)
            {
                return Client.SendStickerAsync(message.Chat, DontUnderstandSticker);
            }

            long userId = message.From.Id;
            switch (command?.Access)
            {
                case null:
                    return Client.SendStickerAsync(message.Chat, DontUnderstandSticker);
                case AccessType.SuperAdmin when !IsSuperAdmin(userId):
                case AccessType.Admins when !IsAdmin(userId) && !IsSuperAdmin(userId):
                    return Client.SendStickerAsync(message.Chat, ForbiddenSticker);
                case AccessType.SuperAdmin:
                case AccessType.Admins:
                case AccessType.Users:
                    return command.ExecuteAsync(message, fromChat, payload);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private async Task UpdateAsync(Message message)
        {
            bool fromChat = message.Chat.Id != message.From.Id;
            string botName = null;
            if (fromChat)
            {
                User user = await GetUserAsunc();
                botName = user.Username;
            }

            foreach (CommandBase<TBot, TConfig> command in Commands)
            {
                if (command.IsInvokingBy(message.Text, out string payload, fromChat, botName))
                {
                    await UpdateAsync(message, fromChat, command, payload);
                    return;
                }
            }

            await UpdateAsync(message, fromChat);
        }

        public readonly TelegramBotClient Client;
        public readonly TConfig Config;
        public readonly TimeManager TimeManager;

        protected readonly List<CommandBase<TBot, TConfig>> Commands;
        protected readonly InputOnlineFile DontUnderstandSticker;
        protected readonly InputOnlineFile ForbiddenSticker;
    }
}