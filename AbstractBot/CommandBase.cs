using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    public abstract class CommandBase<TConfig>
        where TConfig : Config
    {
        protected internal abstract string Name { get; }
        protected internal abstract string Description { get; }

        protected virtual string Alias => null;

        public virtual bool AdminsOnly => false;

        protected CommandBase(BotBase<TConfig> bot) => Bot = bot;

        public virtual bool IsInvokingBy(string text, bool fromChat = false, string botName = null)
        {
            return (fromChat && (text == $"/{Name}@{botName}"))
                   || (!fromChat && ((text == $"/{Name}") || (!string.IsNullOrWhiteSpace(Alias) && (text == Alias))));
        }

        public abstract Task ExecuteAsync(Message message, bool fromChat = false);

        protected readonly BotBase<TConfig> Bot;
    }
}
