using System;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public sealed class TimeManager
{
    internal TimeManager(string timeZoneId) => _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

    public DateTime Now() => ToLocal(DateTime.UtcNow);

    public DateTime ToLocal(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(utc, _timeZoneInfo);

    public DateTime ToUtc(DateTime local) => TimeZoneInfo.ConvertTimeToUtc(local, _timeZoneInfo);

    private readonly TimeZoneInfo _timeZoneInfo;
}
