using System;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations;
using AbstractBot.Models;
using AbstractBot.Models.MessageTemplates;
using AbstractBot.Models.Operations;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations;

[PublicAPI]
public abstract class Operation<T> : Operation
    where T : class
{
    protected Operation(IAccesses accesses, IUpdateSender updateSender, MessageTemplateText? description = null)
        : base(updateSender, description)
    {
        _accesses = accesses;
    }

    internal override async Task<IOperation.ExecutionResult> TryExecuteAsync(Message message, User sender,
        string? callbackQueryData)
    {
        T? data;
        string? callbackQueryDataCore = null;
        if (callbackQueryData is null)
        {
            if (!IsInvokingBy(message, sender, out data))
            {
                return IOperation.ExecutionResult.UnsuitableOperation;
            }
        }
        else
        {
            if (!callbackQueryData.StartsWith(GetType().Name, StringComparison.InvariantCulture))
            {
                return IOperation.ExecutionResult.UnsuitableOperation;
            }

            callbackQueryDataCore = callbackQueryData[GetType().Name.Length..];
            if (!IsInvokingBy(message, sender, callbackQueryDataCore, out data))
            {
                return IOperation.ExecutionResult.UnsuitableOperation;
            }
        }

        AccessData.Status status = _accesses.GetAccess(sender.Id).CheckAgainst(AccessRequired);
        switch (status)
        {
            case AccessData.Status.Insufficient: return IOperation.ExecutionResult.AccessInsufficent;
            case AccessData.Status.Expired: return IOperation.ExecutionResult.AccessExpired;
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
        return IOperation.ExecutionResult.Success;
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

    protected virtual Task ExecuteAsync(Message message, User sender)
    {
        return Task.CompletedTask;
    }

    protected virtual Task ExecuteAsync(T data, Message message, User sender) => ExecuteAsync(message, sender);

    protected virtual Task ExecuteAsync(Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(message, sender);
    }

    protected virtual Task ExecuteAsync(T data, Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(data, message, sender);
    }

    private readonly IAccesses _accesses;
}