using AbstractBot.Legacy.Configs.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations;

[PublicAPI]
public abstract class OperationSimple : Operation<object>
{
    protected OperationSimple(MessageTemplateText? description = null) : base(description) { }

    protected override bool IsInvokingBy(User self, Message message, User sender, out object? data)
    {
        data = null;
        return IsInvokingBy(self, message, sender);
    }

    protected override bool IsInvokingBy(User self, Message message, User sender, string callbackQueryDataCore,
        out object? data)
    {
        data = null;
        return IsInvokingBy(self, message, sender, callbackQueryDataCore);
    }

    protected virtual bool IsInvokingBy(User self, Message message, User sender) => true;

    protected virtual bool IsInvokingBy(User self, Message message, User sender, string callbackQueryDataCore)
    {
        return false;
    }
}