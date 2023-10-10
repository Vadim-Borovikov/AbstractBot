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
        T? data;
        string? callbackQueryDataCore = null;
        if (callbackQueryData is null)
        {
            if (!IsInvokingBy(message, sender, out data))
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
            if (!IsInvokingBy(message, sender, callbackQueryDataCore, out data))
            {
                return ExecutionResult.UnsuitableOperation;
            }
        }

        AccessData.Status status = Bot.GetAccess(sender.Id).CheckAgainst(AccessRequired);
        switch (status)
        {
            case AccessData.Status.Insufficient: return ExecutionResult.AccessInsufficent;
            case AccessData.Status.Expired: return ExecutionResult.AccessExpired;
        }

        if (callbackQueryDataCore is null)
        {
            if (data is null)
            {
                await ExecuteAsync(message, sender);
            }
            else
            {
                await ExecuteAsync(data, message, sender);
            }
        }
        else
        {
            if (data is null)
            {
                await ExecuteAsync(message, sender, callbackQueryDataCore);
            }
            else
            {
                await ExecuteAsync(data, message, sender, callbackQueryDataCore);
            }
        }
        return ExecutionResult.Success;
    }

    protected virtual bool IsInvokingBy(Message message, User sender, out T? data)
    {
        data = null;
        return true;
    }

    protected virtual bool IsInvokingBy(Message message, User sender, string callbackQueryDataCore, out T? data)
    {
        data = null;
        return false;
    }

    protected virtual Task ExecuteAsync(Message message, User sender) => Task.CompletedTask;
    protected virtual Task ExecuteAsync(T data, Message message, User sender) => ExecuteAsync(message, sender);
    protected virtual Task ExecuteAsync(Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(message, sender);
    }

    protected virtual Task ExecuteAsync(T data, Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(data, message, sender);
    }
}