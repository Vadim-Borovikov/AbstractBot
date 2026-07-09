using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities;
using GryphonUtilities.Extensions;
using GryphonUtilities.Logging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace AbstractBot.Modules;

[PublicAPI]
public class UpdateReceiver : IUpdateReceiver
{
    public Logger Logger => _logger;
    public List<IOperation> Operations { get; }

    public UpdateReceiver(IWrongOperationProcessor wrongOperationProcessor, IUpdateSender sender, long selfId,
        LoggerExtended logger)
    {
        _wrongOperationProcessor = wrongOperationProcessor;
        _selfId = selfId;
        _logger = logger;
        _updatesSender = sender;
        Operations = new List<IOperation>();

        UnboundedChannelOptions options = new()
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        };
        _updates = Channel.CreateUnbounded<Update>(options);

        Invoker.FireAndForget(_ => ProcessQueueAsync(), _logger);
    }

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

    protected virtual Task ProcessUnclearOperationAsync(Message message, User? user)
    {
        return _wrongOperationProcessor.ProcessUnclearOperationAsync(_updatesSender, message, user);
    }

    protected virtual Task ProcessInsufficientAccessAsync(Message message, User user, IOperation operation)
    {
        return _wrongOperationProcessor.ProcessInsufficientAccessAsync(_updatesSender, message, user, operation);
    }

    protected virtual Task ProcessExpiredAccess(Message message, User user, IOperation operation)
    {
        return _wrongOperationProcessor.ProcessExpiredAccessAsync(_updatesSender, message, user, operation);
    }

    public void Update(Update update)
    {
        bool written = _updates.Writer.TryWrite(update);
        if (!written)
        {
            throw new InvalidOperationException("Failed to enqueue update.");
        }
    }

    private async Task ProcessQueueAsync()
    {
        await foreach (Update update in _updates.Reader.ReadAllAsync())
        {
            try
            {
                await UpdateAsync(update);
            }
            catch (Exception ex)
            {
                Logger.Errors.Log(ex);
            }
        }
    }

    private readonly Channel<Update> _updates;
    private readonly LoggerExtended _logger;
    private readonly IWrongOperationProcessor _wrongOperationProcessor;
    private readonly IUpdateSender _updatesSender;
    private readonly long _selfId;
}