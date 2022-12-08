using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

public abstract class Operation : IComparable<Operation>
{
    public enum Access
    {
        Users,
        Admins,
        SuperAdmin
    }

    public enum ExecutionResult
    {
        UnsuitableOperation,
        InsufficentAccess,
        Success
    }

    protected internal virtual Access AccessLevel => Access.Users;

    protected bool IsAccessSuffice(long userId) => BotBase.GetMaximumAccessFor(userId) >= AccessLevel;

    protected internal abstract Task<ExecutionResult> TryExecuteAsync(Message message, Chat sender);

    protected internal abstract byte MenuOrder { get; }

    public string? MenuDescription { get; protected init; }

    protected Operation(BotBase bot) => BotBase = bot;

    protected readonly BotBase BotBase;

    public int CompareTo(Operation? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null)
        {
            return 1;
        }

        return MenuOrder.CompareTo(other.MenuOrder);
    }
}