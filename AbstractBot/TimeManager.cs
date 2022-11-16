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

    public DateTimeOffset Now() => ToLocal(DateTimeOffset.UtcNow);

    public DateTimeOffset ToLocal(DateTimeOffset utc)
    {
        DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(utc.UtcDateTime, _timeZoneInfo);
        return new DateTimeOffset(dateTime, _timeZoneInfo.GetUtcOffset(utc));
    }

    public static TimeSpan? GetDelayUntil(DateTimeOffset? start, TimeSpan delay, DateTimeOffset now)
    {
        if (start is null)
        {
            return null;
        }

        DateTimeOffset time = start.Value + delay;
        return time <= now ? null : time - now;
    }

    private readonly TimeZoneInfo _timeZoneInfo;
}
