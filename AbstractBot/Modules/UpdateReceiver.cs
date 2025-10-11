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
    public Logger Logger => _logger;
    public List<IOperation> Operations { get; }

    public UpdateReceiver(InputFileId dontUnderstandSticker, InputFileId forbiddenSticker, long selfId,
        Chat reportsDefault, IUpdateSender sender, LoggerExtended logger)
    {
        _dontUnderstandSticker = dontUnderstandSticker;
        _forbiddenSticker = forbiddenSticker;
        _selfId = selfId;
        _logger = logger;
        _updatesSender = sender;
        _reportsDefault = reportsDefault;
        Operations = new List<IOperation>();
    }

    public void Update(Update update) => Invoker.FireAndForget(_ => UpdateAsync(update), Logger);

    protected virtual Task UpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.ChannelPost   => UpdateAsync(update.ChannelPost.Denull(nameof(update.ChannelPost)), null),
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

        return UpdateAsync(callbackQuery.Message, callbackQuery.From, callbackQuery);
    }

    protected virtual Task UpdateAsync(PreCheckoutQuery _) => Task.CompletedTask;

    protected virtual async Task<IOperation?> UpdateAsync(Message message, User? from,
        CallbackQuery? callbackQuery = null)
    {
        if (from?.Id == _selfId)
        {
            return null;
        }

        if (callbackQuery is null)
        {
            _logger.LogUpdate(message.Chat, LoggerExtended.UpdateType.ReceiveMessage, message.MessageId,
                $"{message.Text}{message.Caption}");
        }
        else
        {
            _logger.LogUpdate(message.Chat, LoggerExtended.UpdateType.ReceiveCallback, message.MessageId,
                callbackQuery.Data);
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

            IOperation.ExecutionResult result = await operation.TryExecuteAsync(message, from, callbackQuery);
            switch (result)
            {
                case IOperation.ExecutionResult.UnsuitableOperation: continue;
                case IOperation.ExecutionResult.AccessInsufficent:
                    if (from is null)
                    {
                        throw new Exception("Operation returned AccessInsufficient with null From");
                    }
                    await ProcessInsufficientAccessAsync(message, from, operation);
                    return operation;
                case IOperation.ExecutionResult.AccessExpired:
                    if (from is null)
                    {
                        throw new Exception("Operation returned AccessExpired with null From");
                    }
                    await ProcessExpiredAccess(message, from, operation);
                    return operation;
                case IOperation.ExecutionResult.Success: return operation;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        await ProcessUnclearOperationAsync(message, from);
        return null;
    }

    protected virtual Task ProcessUnclearOperationAsync(Message message, User? _)
    {
        return SendStickerAsync(message, _dontUnderstandSticker);
    }

    protected virtual Task ProcessInsufficientAccessAsync(Message message, User _, IOperation __)
    {
        return SendStickerAsync(message, _forbiddenSticker);
    }

    protected virtual Task ProcessExpiredAccess(Message message, User _, IOperation __)
    {
        return ProcessInsufficientAccessAsync(message, _, __);
    }

    private Task SendStickerAsync(Message message, InputFile sticker)
    {
        Chat chat = message.Chat;
        ReplyParameters rp = new() { MessageId = message.MessageId };
        if (message.Chat.IsGroup() || (message.Chat.Type == ChatType.Channel))
        {
            chat = _reportsDefault;
            rp.ChatId = message.Chat.Id;
        }

        return _updatesSender.SendStickerAsync(chat, sticker, rp);
    }

    private readonly LoggerExtended _logger;
    private readonly IUpdateSender _updatesSender;
    private readonly InputFileId _dontUnderstandSticker;
    private readonly InputFileId _forbiddenSticker;
    private readonly long _selfId;
    private readonly Chat _reportsDefault;
}