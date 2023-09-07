using AbstractBot.Bots;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

[PublicAPI]
public abstract class OperationSimple : Operation<object>
{
    protected OperationSimple(BotBasic bot) : base(bot) { }

    protected override bool IsInvokingBy(Message message, User sender, out object? info)
    {
        info = null;
        return IsInvokingBy(message, sender);
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore, out object? info)
    {
        info = null;
        return IsInvokingBy(message, sender, callbackQueryDataCore);
    }

    protected virtual bool IsInvokingBy(Message message, User sender) => true;

    protected virtual bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore) => false;
}