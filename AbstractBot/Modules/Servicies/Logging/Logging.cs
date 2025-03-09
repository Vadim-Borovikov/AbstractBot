using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules.Servicies;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Servicies.Logging;

[PublicAPI]
public class Logging : ILogging, IDisposable
{
    public LoggerExtended Logger { get; }

    public Logging(LoggerExtended logger, TimeSpan tickInterval)
    {
        Logger = logger;
        _ticker = new Ticker(logger, tickInterval);
    }

    public void Dispose() => _ticker.Dispose();

    public Task StartAsync(CancellationToken cancellationToken) => _ticker.StartAsync(cancellationToken);
    public Task StopAsync(CancellationToken cancellationToken) => _ticker.StopAsync(cancellationToken);

    private readonly Ticker _ticker;
}