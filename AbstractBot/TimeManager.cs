using System;
using System.Diagnostics.CodeAnalysis;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class TimeManager
    {
        internal TimeManager(string timeZoneId)
        {
            _timeZoneInfo =
                string.IsNullOrWhiteSpace(timeZoneId) ? null : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public DateTime Now() => ToLocal(DateTime.UtcNow);

        public DateTime ToLocal(DateTime utc)
        {
            return _timeZoneInfo == null
                ? utc.ToLocalTime()
                : TimeZoneInfo.ConvertTimeFromUtc(utc, _timeZoneInfo);
        }

        public DateTime ToUtc(DateTime local)
        {
            return _timeZoneInfo == null
                ? local.ToUniversalTime()
                : TimeZoneInfo.ConvertTimeToUtc(local, _timeZoneInfo);
        }

        private readonly TimeZoneInfo _timeZoneInfo;
    }
}
