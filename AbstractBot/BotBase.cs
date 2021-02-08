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
    [SuppressMessage("ReSharper", "UnassignedReadonlyField")]
    public abstract class BotBase<TBot, TConfig>
        where TBot: BotBase<TBot, TConfig>
        where TConfig: Config
    {
        protected BotBase(TConfig config)
        {
            Config = config;

            Client = new TelegramBotClient(Config.Token);

            Commands = new List<CommandBase<TBot, TConfig>>();

            DontUnderstandSticker = new InputOnlineFile(Config.DontUnderstandStickerFileId);
            ForbiddenSticker = new InputOnlineFile(Config.ForbiddenStickerFileId);

            Utils.SetupTimeZoneInfo(Config.SystemTimeZoneId);
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Client.SetWebhookAsync(Config.Url, cancellationToken: cancellationToken);
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Client.DeleteWebhookAsync(cancellationToken);
        }

        public Task UpdateAsync(Update update)
        {
            return update?.Type == UpdateType.Message ? UpdateAsync(update.Message) : Task.CompletedTask;
        }

        public Task<User> GetUserAsunc() => Client.GetMeAsync();

        public bool FromAdmin(Message message)
        {
            return (Config.AdminIds != null) && Config.AdminIds.Contains(message.From.Id);
        }

        public string GetDescription(bool fromAdmin = false)
        {
            var builder = new StringBuilder();

            if (Config.DescriptionStart?.Count > 0)
            {
                foreach (string line in Config.DescriptionStart)
                {
                    builder.AppendLine(line);
                }
            }

            List<CommandBase<TBot, TConfig>> userCommands = Commands.Where(c => !c.AdminsOnly).ToList();
            if (fromAdmin)
            {
                List<CommandBase<TBot, TConfig>> adminCommands = Commands.Where(c => c.AdminsOnly).ToList();
                if (adminCommands.Count > 0)
                {
                    builder.AppendLine(adminCommands.Count > 1 ? "Админские команды:" : "Админская команда:");
                    foreach (CommandBase<TBot, TConfig> command in adminCommands)
                    {
                        builder.AppendLine($"/{command.Name} – {command.Description}");
                    }
                    if (userCommands.Count > 0)
                    {
                        builder.AppendLine();
                    }
                }
            }

            if (userCommands.Count > 0)
            {
                builder.AppendLine(userCommands.Count > 1 ? "Команды:" : "Команда:");
                foreach (CommandBase<TBot, TConfig> command in userCommands)
                {
                    builder.AppendLine($"/{command.Name} – {command.Description}");
                }
            }

            if (Config.DescriptionEnd?.Count > 0)
            {
                foreach (string line in Config.DescriptionEnd)
                {
                    builder.AppendLine(line);
                }
            }

            return builder.ToString();
        }

        protected virtual Task UpdateAsync(Message message, CommandBase<TBot, TConfig> command, bool fromChat = false)
        {
            if (command == null)
            {
                return Client.SendStickerAsync(message.Chat, DontUnderstandSticker);
            }

            if (command.AdminsOnly && !FromAdmin(message))
            {
                return Client.SendStickerAsync(message.Chat, ForbiddenSticker);
            }

            return command.ExecuteAsync(message, fromChat);
        }

        protected virtual async Task UpdateAsync(Message message)
        {
            if (message.Type != MessageType.Text)
            {
                await Client.SendStickerAsync(message.Chat, DontUnderstandSticker);
                return;
            }

            bool fromChat = message.Chat.Id != message.From.Id;
            string botName = null;
            if (fromChat)
            {
                User user = await GetUserAsunc();
                botName = user.Username;
            }
            CommandBase<TBot, TConfig> command =
                Commands.FirstOrDefault(c => c.IsInvokingBy(message.Text, fromChat, botName));
            await UpdateAsync(message, command, fromChat);
        }

        public readonly TelegramBotClient Client;
        public readonly TConfig Config;

        protected readonly List<CommandBase<TBot, TConfig>> Commands;
        protected readonly InputOnlineFile DontUnderstandSticker;
        protected readonly InputOnlineFile ForbiddenSticker;
    }
}