using System;
using System.Timers;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public class TickManager : IDisposable
{
    public TickManager()
    {
        _timer = new Timer
        {
            Interval = TickPeriod.TotalMilliseconds,
            AutoReset = true
        };
        _timer.Elapsed += (_, _) => OnTimerElapsed();
    }

    public void Start()
    {
        Tick();
        _timer.Start();
    }

    public void Stop() => _timer.Stop();

    public void Dispose() => _timer.Dispose();

    private void Tick() => Utils.LogManager.LogTimedMessage("Tick");

    private void OnTimerElapsed()
    {
        try
        {
            Tick();
        }
        catch (Exception ex)
        {
            Utils.LogManager.LogException(ex);
        }
    }

    private readonly Timer _timer;
    private static readonly TimeSpan TickPeriod = TimeSpan.FromSeconds(30);
}
