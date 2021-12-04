using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global")]
    public abstract class CommandBase<TBot, TConfig>
        where TBot : BotBase<TBot, TConfig>
        where TConfig : Config
    {
        protected internal abstract string Name { get; }
        protected internal abstract string Description { get; }

        protected virtual string Alias => null;

        public virtual BotBase<TBot, TConfig>.AccessType Access => BotBase<TBot, TConfig>.AccessType.Users;

        protected CommandBase(TBot bot) => Bot = bot;

        public virtual bool IsInvokingBy(string text, out string payload, bool fromChat = false, string botName = null)
        {
            if (!fromChat)
            {
                payload = Utils.GetPostfix(text, $"/{Name} ");
                if (!string.IsNullOrWhiteSpace(payload))
                {
                    return true;
                }
            }

            payload = null;
            return (fromChat && (text == $"/{Name}@{botName}"))
                   || (!fromChat && ((text == $"/{Name}") || (!string.IsNullOrWhiteSpace(Alias) && (text == Alias))));
        }

        public abstract Task ExecuteAsync(Message message, bool fromChat, string payload);

        protected readonly TBot Bot;
    }
}
