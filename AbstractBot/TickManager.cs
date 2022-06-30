using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbstractBot;

internal static class TickManager
{
    public static void Start(CancellationToken cancellationToken)
    {
        Invoker.DoPeriodically(TickAsync, Interval, true, cancellationToken);
    }

    private static Task TickAsync(CancellationToken _)
    {
        Utils.LogManager.LogTimedMessage("Tick");
        return Task.CompletedTask;
    }

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);
}