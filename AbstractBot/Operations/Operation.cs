using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

[PublicAPI]
public abstract class Operation<T> : OperationBasic
    where T : class
{
    protected Operation(BotBasic bot) : base(bot) { }

    internal override async Task<ExecutionResult> TryExecuteAsync(Message message, User sender,
        string? callbackQueryData)
    {
        if (AccessLevel > Bot.GetMaximumAccessFor(sender.Id))
        {
            return ExecutionResult.InsufficentAccess;
        }

        T? info;
        string? callbackQueryDataCore = null;
        if (callbackQueryData is null)
        {
            if (!IsInvokingBy(message, sender, out info))
            {
                return ExecutionResult.UnsuitableOperation;
            }
        }
        else
        {
            if (!callbackQueryData.StartsWith(GetType().Name, StringComparison.InvariantCulture))
            {
                return ExecutionResult.UnsuitableOperation;
            }

            callbackQueryDataCore = callbackQueryData[GetType().Name.Length..];
            if (!IsInvokingBy(message, sender, callbackQueryDataCore, out info))
            {
                return ExecutionResult.UnsuitableOperation;
            }
        }

        if (callbackQueryDataCore is null)
        {
            if (info is null)
            {
                await ExecuteAsync(message, sender);
            }
            else
            {
                await ExecuteAsync(info, message, sender);
            }
        }
        else
        {
            if (info is null)
            {
                await ExecuteAsync(message, sender, callbackQueryDataCore);
            }
            else
            {
                await ExecuteAsync(info, message, sender, callbackQueryDataCore);
            }
        }
        return ExecutionResult.Success;
    }

    protected virtual bool IsInvokingBy(Message message, User sender, out T? info)
    {
        info = null;
        return true;
    }

    protected virtual bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore, out T? info)
    {
        info = null;
        return false;
    }

    protected virtual Task ExecuteAsync(Message message, User sender) => Task.CompletedTask;
    protected virtual Task ExecuteAsync(T info, Message message, User sender) => ExecuteAsync(message, sender);
    protected virtual Task ExecuteAsync(Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(message, sender);
    }

    protected virtual Task ExecuteAsync(T info, Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(info, message, sender);
    }
}