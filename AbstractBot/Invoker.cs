using System;
using System.Threading;
using System.Threading.Tasks;
using GryphonUtilities;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public static class Invoker
{
    public static void DoAfterDelay(Func<CancellationToken, Task> doWork, TimeSpan delay, Logger logger,
        CancellationToken cancellationToken)
    {
        FireAndForget(ct => DoAfterDelayAsync(doWork, delay, ct), logger, cancellationToken);
    }

    public static void DoPeriodically(Func<CancellationToken, Task> doWork, TimeSpan interval, bool doNow,
        Logger logger, CancellationToken cancellationToken)
    {
        FireAndForget(ct => DoPeriodicallyAsync(doWork, interval, doNow, ct), logger, cancellationToken);
    }

    public static void FireAndForget(Func<CancellationToken, Task> doWork, Logger logger,
        CancellationToken cancellationToken = default)
    {
        Task.Run(() => doWork(cancellationToken), cancellationToken)
            .ContinueWith(logger.LogExceptionIfPresents, cancellationToken);
    }

    private static async Task DoAfterDelayAsync(Func<CancellationToken, Task> doWork, TimeSpan delay,
        CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        await doWork(cancellationToken);
    }

    private static async Task DoPeriodicallyAsync(Func<CancellationToken, Task> doWork, TimeSpan interval, bool doNow,
        CancellationToken cancellationToken)
    {
        if (doNow)
        {
            await doWork(cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested)
        {
            await DoAfterDelayAsync(doWork, interval, cancellationToken);
        }
    }
}
