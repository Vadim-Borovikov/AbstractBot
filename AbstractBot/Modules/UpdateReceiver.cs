using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace AbstractBot.Modules;

[PublicAPI]
public class UpdateReceiver : IUpdateReceiver
{
    public Logger Logger => _logging.Logger;
    public List<IOperation> Operations { get; }

    public UpdateReceiver(InputFileId dontUnderstandSticker, InputFileId forbiddenSticker, long selfId,
        IUpdateSender sender, ILogging logging)
    {
        _dontUnderstandSticker = dontUnderstandSticker;
        _forbiddenSticker = forbiddenSticker;
        _selfId = selfId;
        _logging = logging;
        _updatesSender = sender;
        Operations = new List<IOperation>();
    }

    public void Update(Update update) => Invoker.FireAndForget(_ => UpdateAsync(update), Logger);

    protected virtual Task UpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message       => UpdateAsync(update.Message.Denull(nameof(update.Message))),
            UpdateType.CallbackQuery => UpdateAsync(update.CallbackQuery.Denull(nameof(update.CallbackQuery))),
            UpdateType.PreCheckoutQuery =>
                UpdateAsync(update.PreCheckoutQuery.Denull(nameof(update.PreCheckoutQuery))),
            _ => Task.CompletedTask
        };
    }

    protected virtual async Task UpdateAsync(Message message)
    {
        if (message.From is null)
        {
            throw new Exception("Message update with null From");
        }

        await UpdateAsync(message, message.From);
    }

    protected virtual Task UpdateAsync(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Message is null)
        {
            throw new Exception("CallbackQuery update with null Message");
        }

        if (string.IsNullOrWhiteSpace(callbackQuery.Data))
        {
            throw new Exception("CallbackQuery update with null Data");
        }

        return UpdateAsync(callbackQuery.Message, callbackQuery.From, callbackQuery.Data);
    }

    protected virtual Task UpdateAsync(PreCheckoutQuery _) => Task.CompletedTask;

    protected virtual async Task<IOperation?> UpdateAsync(Message message, User from,
        string? callbackQueryData = null)
    {
        if (from.Id == _selfId)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(callbackQueryData))
        {
            _logging.LogUpdate(message.Chat, Logging.UpdateType.ReceiveMessage, message.MessageId,
                $"{message.Text}{message.Caption}");
        }
        else
        {
            _logging.LogUpdate(message.Chat, Logging.UpdateType.ReceiveCallback, message.MessageId, callbackQueryData);
        }

        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (IOperation operation in Operations)
        {
            if (message.Chat.IsGroup() && !operation.EnabledInGroups)
            {
                continue;
            }

            if ((message.Chat.Type == ChatType.Channel) && !operation.EnabledInChannels)
            {
                continue;
            }

            IOperation.ExecutionResult result = await operation.TryExecuteAsync(message, from, callbackQueryData);
            switch (result)
            {
                case IOperation.ExecutionResult.UnsuitableOperation: continue;
                case IOperation.ExecutionResult.AccessInsufficent:
                    await ProcessInsufficientAccess(message, from, operation);
                    return operation;
                case IOperation.ExecutionResult.AccessExpired:
                    await ProcessExpiredAccess(message, from, operation);
                    return operation;
                case IOperation.ExecutionResult.Success: return operation;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        await ProcessUnclearOperation(message, from);
        return null;
    }

    protected virtual Task ProcessUnclearOperation(Message message, User _)
    {
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return message.Chat.IsGroup()
            ? Task.CompletedTask
            : _updatesSender.SendStickerAsync(message.Chat, _dontUnderstandSticker, rp);
    }

    protected virtual Task ProcessInsufficientAccess(Message message, User _, IOperation __)
    {
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return message.Chat.IsGroup()
            ? Task.CompletedTask
            : _updatesSender.SendStickerAsync(message.Chat, _forbiddenSticker, rp);
    }

    protected virtual Task ProcessExpiredAccess(Message message, User _, IOperation __)
    {
        return ProcessInsufficientAccess(message, _, __);
    }

    private readonly ILogging _logging;
    private readonly IUpdateSender _updatesSender;
    private readonly InputFileId _dontUnderstandSticker;
    private readonly InputFileId _forbiddenSticker;
    private readonly long _selfId;
}