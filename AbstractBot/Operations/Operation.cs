using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

public abstract class Operation : IComparable<Operation>
{
    public enum Access
    {
        User,
        Admin,
        SuperAdmin
    }

    public enum ExecutionResult
    {
        UnsuitableOperation,
        InsufficentAccess,
        Success
    }

    [PublicAPI]
    protected internal virtual Access AccessLevel => Access.User;

    [PublicAPI]
    protected internal virtual bool EnabledInGroups => false;

    protected internal string? MenuDescription { get; protected init; }

    protected abstract byte MenuOrder { get; }

    protected readonly Bot Bot;

    protected Operation(Bot bot) => Bot = bot;

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

    protected internal abstract Task<ExecutionResult> TryExecuteAsync(Message message, long senderId);

    protected bool IsAccessSuffice(long userId) => Bot.GetMaximumAccessFor(userId) >= AccessLevel;
}