using AbstractBot.Bots;
using AbstractBot.Configs.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

[PublicAPI]
public abstract class OperationSimple : Operation<object>
{
    protected OperationSimple(BotBasic bot, MessageTemplateText? description = null) : base(bot, description) { }

    protected override bool IsInvokingBy(Message message, User sender, out object? data)
    {
        data = null;
        return IsInvokingBy(message, sender);
    }

    protected override bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore, out object? data)
    {
        data = null;
        return IsInvokingBy(message, sender, callbackQueryDataCore);
    }

    protected virtual bool IsInvokingBy(Message message, User sender) => true;

    protected virtual bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore) => false;
}