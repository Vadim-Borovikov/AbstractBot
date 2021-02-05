using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Commands
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    public abstract class CommandBase
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        protected internal virtual bool IsInvokingBy(string text, bool fromChat, string botName)
        {
            return text == (fromChat ? $"/{Name}@{botName}" : $"/{Name}");
        }

        public abstract Task ExecuteAsync(ChatId chatId, ITelegramBotClient client, int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null);
    }
}
