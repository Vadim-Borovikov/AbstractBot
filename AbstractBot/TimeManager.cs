using System;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public sealed class TimeManager
{
    internal TimeManager(string? timeZoneId = null)
    {
        _timeZoneInfo = timeZoneId is null ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public DateTime Now() => ToLocal(DateTime.UtcNow);

    public DateTime ToLocal(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(utc, _timeZoneInfo);

    public DateTime ToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(local, _timeZoneInfo);

    public static TimeSpan? GetDelayUntil(DateTime? start, TimeSpan delay, DateTime now)
    {
        if (start is null)
        {
            return null;
        }

        DateTime time = start.Value + delay;
        return time <= now ? null : time - now;
    }

    private readonly TimeZoneInfo _timeZoneInfo;
}
