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
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    public abstract class BotBase<TConfig> : IDescriptionProvider
        where TConfig: Config
    {
        protected BotBase(TConfig config)
        {
            Config = config;

            Client = new TelegramBotClient(Config.Token);

            Commands = new List<CommandBase>();

            DontUnderstandSticker = new InputOnlineFile(Config.DontUnderstandStickerFileId);
            ForbiddenSticker = new InputOnlineFile(Config.ForbiddenStickerFileId);

            Utils.SetupTimeZoneInfo(Config.SystemTimeZoneId);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Client.SetWebhookAsync(Config.Url, cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Client.DeleteWebhookAsync(cancellationToken);

        public Task UpdateAsync(Update update)
        {
            return update?.Type == UpdateType.Message ? UpdateAsync(update.Message) : Task.CompletedTask;
        }

        public Task<User> GetUserAsunc() => Client.GetMeAsync();

        public bool FromAdmin(Message message)
        {
            return (Config.AdminIds != null) && Config.AdminIds.Contains(message.From.Id);
        }

        public string GetDescription()
        {
            var builder = new StringBuilder();

            if (Config.DescriptionStart?.Count > 0)
            {
                foreach (string line in Config.DescriptionStart)
                {
                    builder.AppendLine(line);
                }
            }

            if (Commands.Count > 0)
            {
                builder.AppendLine(Commands.Count > 1 ? "Команды:" : "Команда:");
                foreach (CommandBase command in Commands)
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

        protected virtual Task UpdateAsync(Message message, CommandBase command, bool fromChat = false)
        {
            if (command == null)
            {
                return Client.SendStickerAsync(message, DontUnderstandSticker);
            }

            if (command.AdminsOnly && !FromAdmin(message))
            {
                return Client.SendStickerAsync(message, ForbiddenSticker);
            }

            return command.ExecuteAsync(message.Chat, Client);
        }

        protected virtual async Task UpdateAsync(Message message)
        {
            if (message.Type != MessageType.Text)
            {
                await Client.SendStickerAsync(message, DontUnderstandSticker);
                return;
            }

            bool fromChat = message.Chat.Id != message.From.Id;
            string botName = null;
            if (fromChat)
            {
                User user = await GetUserAsunc();
                botName = user.Username;
            }
            CommandBase command = Commands.FirstOrDefault(c => c.IsInvokingBy(message.Text, fromChat, botName));
            await UpdateAsync(message, command, fromChat);
        }

        public readonly TelegramBotClient Client;
        public readonly TConfig Config;

        protected readonly List<CommandBase> Commands;
        protected readonly InputOnlineFile DontUnderstandSticker;
        protected readonly InputOnlineFile ForbiddenSticker;
    }
}