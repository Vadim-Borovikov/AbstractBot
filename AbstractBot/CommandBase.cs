using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    public abstract class CommandBase
    {
        protected internal abstract string Name { get; }
        protected internal abstract string Description { get; }

        protected internal virtual bool IsInvokingBy(string text, bool fromChat, string botName)
        {
            return text == (fromChat ? $"/{Name}@{botName}" : $"/{Name}");
        }

        public abstract Task ExecuteAsync(ChatId chatId, ITelegramBotClient client, int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null);
    }
}
