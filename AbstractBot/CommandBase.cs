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

        protected virtual string Alias => null;

        protected internal virtual bool AdminsOnly => false;

        public virtual bool IsInvokingBy(string text, bool fromChat = false, string botName = null)
        {
            return (fromChat && (text == $"/{Name}@{botName}"))
                   || (!fromChat && ((text == $"/{Name}") || (!string.IsNullOrWhiteSpace(Alias) && (text == Alias))));
        }

        public abstract Task ExecuteAsync(ChatId chatId, ITelegramBotClient client, int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null);
    }
}
