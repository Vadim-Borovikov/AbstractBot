using System;
using System.Threading.Tasks;
using AbstractBot.Legacy.Bots;
using AbstractBot.Legacy.Configs.MessageTemplates;
using AbstractBot.Models;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations;

[PublicAPI]
public abstract class Operation<T> : OperationBasic
    where T : class
{
    protected Operation(MessageTemplateText? description = null) : base(description) { }

    internal override async Task<ExecutionResult> TryExecuteAsync(BotBasic bot, Message message, User sender,
        string? callbackQueryData)
    {
        T? data;
        string? callbackQueryDataCore = null;
        if (callbackQueryData is null)
        {
            if (!IsInvokingBy(bot.Self, message, sender, out data))
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
            if (!IsInvokingBy(bot.Self, message, sender, callbackQueryDataCore, out data))
            {
                return ExecutionResult.UnsuitableOperation;
            }
        }

        AccessData.Status status = bot.GetAccess(sender.Id).CheckAgainst(AccessRequired);
        switch (status)
        {
            case AccessData.Status.Insufficient: return ExecutionResult.AccessInsufficent;
            case AccessData.Status.Expired: return ExecutionResult.AccessExpired;
        }

        if (callbackQueryDataCore is null)
        {
            if (data is null)
            {
                await ExecuteAsync(bot, message, sender);
            }
            else
            {
                await ExecuteAsync(bot, data, message, sender);
            }
        }
        else
        {
            if (data is null)
            {
                await ExecuteAsync(bot, message, sender, callbackQueryDataCore);
            }
            else
            {
                await ExecuteAsync(bot, data, message, sender, callbackQueryDataCore);
            }
        }
        return ExecutionResult.Success;
    }

    protected virtual bool IsInvokingBy(User self, Message message, User sender, out T? data)
    {
        data = null;
        return true;
    }

    protected virtual bool IsInvokingBy(User self, Message message, User sender, string callbackQueryDataCore,
        out T? data)
    {
        data = null;
        return false;
    }

    protected virtual Task ExecuteAsync(BotBasic bot, Message message, User sender) => Task.CompletedTask;
    protected virtual Task ExecuteAsync(BotBasic bot, T data, Message message, User sender)
    {
        return ExecuteAsync(bot, message, sender);
    }

    protected virtual Task ExecuteAsync(BotBasic bot, Message message, User sender, string callbackQueryDataCore)
    {
        return ExecuteAsync(bot, message, sender);
    }

    protected virtual Task ExecuteAsync(BotBasic bot, T data, Message message, User sender,
        string callbackQueryDataCore)
    {
        return ExecuteAsync(bot, data, message, sender);
    }
}