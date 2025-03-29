using System;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations;

[PublicAPI]
public abstract class Operation<TData> : OperationBase
    where TData : class
{
    protected Operation(IAccesses accesses, IUpdateSender updateSender) : base(accesses, updateSender) { }

    public override async Task<IOperation.ExecutionResult> TryExecuteAsync(Message message, User from,
        CallbackQuery? callbackQuery)
    {
        TData? data;
        string? callbackQueryDataCore = null;
        if (callbackQuery?.Data is null)
        {
            if (!IsInvokingBy(message, from, out data))
            {
                return IOperation.ExecutionResult.UnsuitableOperation;
            }
        }
        else
        {
            callbackQueryDataCore = TryGetQueryCore(callbackQuery.Data);
            if (callbackQueryDataCore is null)
            {
                return IOperation.ExecutionResult.UnsuitableOperation;
            }

            if (!IsInvokingBy(message, from, callbackQueryDataCore, out data))
            {
                return IOperation.ExecutionResult.UnsuitableOperation;
            }
        }

        if (callbackQuery is not null)
        {
            await UpdateSender.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        AccessData.Status status = CheckAccess(from.Id);
        switch (status)
        {
            case AccessData.Status.Insufficient: return IOperation.ExecutionResult.AccessInsufficent;
            case AccessData.Status.Expired: return IOperation.ExecutionResult.AccessExpired;
            case AccessData.Status.Sufficient:
                if (callbackQueryDataCore is null)
                {
                    if (data is null)
                    {
                        await ExecuteAsync(message, from);
                    }
                    else
                    {
                        await ExecuteAsync(data, message, from);
                    }
                }
                else
                {
                    if (data is null)
                    {
                        await ExecuteAsync(message, from, callbackQueryDataCore);
                    }
                    else
                    {
                        await ExecuteAsync(data, message, from, callbackQueryDataCore);
                    }
                }
                return IOperation.ExecutionResult.Success;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual bool IsInvokingBy(Message message, User from, out TData? data)
    {
        data = null;
        return false;
    }

    protected virtual bool IsInvokingBy(Message message, User from, string callbackQueryDataCore, out TData? data)
    {
        data = null;
        return false;
    }

    protected virtual Task ExecuteAsync(TData data, Message message, User from) => Task.CompletedTask;

    protected virtual Task ExecuteAsync(TData data, Message message, User from, string callbackQueryDataCore)
    {
        return ExecuteAsync(data, message, from);
    }
}