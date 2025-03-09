using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules.Servicies;
using GryphonUtilities;

namespace AbstractBot.Modules.Servicies.Logging;

internal sealed class Ticker : IDisposable, IService
{
    public Ticker(Logger logger, TimeSpan interval)
    {
        _logger = logger;
        _interval = interval;
        _cancellationSource = new CancellationTokenSource();
    }

    public void Dispose() => _cancellationSource.Dispose();

    public Task StartAsync(CancellationToken _)
    {
        Invoker.DoPeriodically(TickAsync, _interval, true, _logger, _cancellationSource.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken _) => _cancellationSource.CancelAsync();

    private Task TickAsync(CancellationToken _)
    {
        _logger.LogTimedMessage("Tick");
        return Task.CompletedTask;
    }

    private readonly Logger _logger;
    private readonly TimeSpan _interval;
    private readonly CancellationTokenSource _cancellationSource;
}