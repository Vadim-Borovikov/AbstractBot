using System;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public sealed class TimeManager
{
    internal TimeManager(string? timeZoneId)
    {
        _timeZoneInfo = string.IsNullOrWhiteSpace(timeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public DateTime Now() => ToLocal(DateTime.UtcNow);

    public DateTime ToLocal(DateTime utc)
    {
        return _timeZoneInfo is null ? utc.ToLocalTime() : TimeZoneInfo.ConvertTimeFromUtc(utc, _timeZoneInfo);
    }

    public DateTime ToUtc(DateTime local)
    {
        return _timeZoneInfo is null ? local.ToUniversalTime() : TimeZoneInfo.ConvertTimeToUtc(local, _timeZoneInfo);
    }

    private readonly TimeZoneInfo? _timeZoneInfo;
}