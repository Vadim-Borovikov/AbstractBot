using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbstractBot;

internal sealed class TickManager : IntervalInvoker
{
    public TickManager() : base(TickAsync, Interval) { }

    private static Task TickAsync(CancellationToken _)
    {
        Utils.LogManager.LogTimedMessage("Tick");
        return Task.CompletedTask;
    }

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);
}