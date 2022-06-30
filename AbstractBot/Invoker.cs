using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public static class Invoker
{
    public static void DoAfterDelay(Func<CancellationToken, Task> doWork, TimeSpan delay,
        CancellationToken cancellationToken)
    {
        Utils.FireAndForget(ct => DoAfterDelayAsync(doWork, delay, ct), cancellationToken);
    }

    public static void DoPeriodically(Func<CancellationToken, Task> doWork, TimeSpan interval, bool doNow,
        CancellationToken cancellationToken)
    {
        Utils.FireAndForget(ct => DoPeriodicallyAsync(doWork, interval, doNow, ct), cancellationToken);
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
