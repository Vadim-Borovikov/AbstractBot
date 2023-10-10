using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

[PublicAPI]
public abstract class OperationBasic : IComparable<OperationBasic>
{
    internal enum ExecutionResult
    {
        UnsuitableOperation,
        AccessInsufficent,
        AccessExpired,
        Success
    }

    public virtual Enum? AccessRequired => null;

    protected internal virtual bool EnabledInGroups => false;
    protected internal virtual bool EnabledInChannels => false;

    protected internal string? MenuDescription { get; protected init; }

    protected virtual byte Order => 0;

    protected readonly BotBasic Bot;

    protected OperationBasic(BotBasic bot) => Bot = bot;

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

    internal abstract Task<ExecutionResult> TryExecuteAsync(Message message, User sender, string? callbackQueryData);
}