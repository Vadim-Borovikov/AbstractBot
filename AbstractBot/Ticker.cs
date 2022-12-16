using System;
using System.Threading;
using System.Threading.Tasks;
using GryphonUtilities;

namespace AbstractBot;

internal sealed class Ticker
{
    public Ticker(Logger logger) => _logger = logger;

    public void Start(CancellationToken cancellationToken)
    {
        Invoker.DoPeriodically(TickAsync, Interval, true, _logger, cancellationToken);
    }

    private Task TickAsync(CancellationToken _)
    {
        _logger.LogTimedMessage("Tick");
        return Task.CompletedTask;
    }

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    private readonly Logger _logger;
}