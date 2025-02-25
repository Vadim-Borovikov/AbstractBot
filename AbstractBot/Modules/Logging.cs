using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using GryphonUtilities;
using GryphonUtilities.Time;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules;

[PublicAPI]
public class Logging : ILogging, IDisposable
{
    public enum UpdateType
    {
        SendText,
        EditText,
        EditMedia,
        Delete,
        Forward,
        SendPhoto,
        SendSticker,
        Pin,
        Unpin,
        UnpinAll,
        SendInvoice,
        SendFiles,
        ReceiveMessage,
        ReceiveCallback
    }

    public Logger Logger { get; }

    public Logging(Clock clock, TimeSpan tickInterval)
    {
        Logger = new Logger(clock);
        _tickInterval = tickInterval;
        _tickCancellationSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        if (!_tickCancellationSource.IsCancellationRequested)
        {
            _tickCancellationSource.Cancel();
        }
        _tickCancellationSource.Dispose();
    }

    public Task StartAsync(CancellationToken _)
    {
        Invoker.DoPeriodically(TickAsync, _tickInterval, true, Logger, _tickCancellationSource.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken _) => _tickCancellationSource.CancelAsync();

    public void LogUpdateRefused(Chat chat, Enum type, int? messageId = null,
        string? data = null)
    {
        string log = GetUpdateLog(chat, type, messageId, data);
        Logger.LogTimedMessage($"Refuse to {log}");
    }

    public void LogUpdate(Chat chat, Enum type, int? messageId = null, string? data = null)
    {
        string log = GetUpdateLog(chat, type, messageId, data);
        Logger.LogTimedMessage(log);
    }

    private string GetUpdateLog(Chat chat, Enum type, int? messageId = null, string? data = null)
    {
        string? messageIdPart = messageId is null ? null : $"message {messageId} ";
        string? dataPart = data is null ? null : $"\"{data.ReplaceLineEndings().Replace(Environment.NewLine, "↵")}\" ";
        return $"{chat.Type} chat {chat.Id}: {type} {messageIdPart}{dataPart}";
    }

    private Task TickAsync(CancellationToken _)
    {
        Logger.LogTimedMessage("Tick");
        return Task.CompletedTask;
    }

    private readonly TimeSpan _tickInterval;
    private readonly CancellationTokenSource _tickCancellationSource;
}