using System;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Operations;
using AbstractBot.Legacy.Bots;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations;

[PublicAPI]
public abstract class OperationBasic : IComparable<OperationBasic>
{
    public virtual Enum? AccessRequired => null;

    protected internal virtual bool EnabledInGroups => false;
    protected internal virtual bool EnabledInChannels => false;

    protected internal readonly MessageTemplateText? Description;

    protected virtual byte Order => 0;

    protected OperationBasic(MessageTemplateText? description = null) => Description = description;

    public int CompareTo(OperationBasic? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        return Order.CompareTo(other.Order);
    }

    internal abstract Task<IOperation.ExecutionResult> TryExecuteAsync(BotBasic bot, Message message, User sender,
        string? callbackQueryData);
}