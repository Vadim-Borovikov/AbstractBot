using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public class IntervalInvoker
{
    public IntervalInvoker(Func<CancellationToken, Task> doWork, TimeSpan interval)
    {
        _doWork = doWork;
        _interval = interval;
    }

    public void Start(CancellationToken cancellationToken)
    {
        Utils.FireAndForget(DoAndRepeatAsync, cancellationToken);
    }

    private async Task DoAndRepeatAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _doWork(cancellationToken);
            await Task.Delay(_interval, cancellationToken);
        }
    }

    private readonly Func<CancellationToken, Task> _doWork;
    private readonly TimeSpan _interval;
}
