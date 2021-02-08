using System;
using System.Diagnostics.CodeAnalysis;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Utils
    {
        public static DateTime Now() => DateTime.UtcNow.ToLocal();

        public static DateTime ToLocal(this DateTime utc)
        {
            return _timeZoneInfo == null
                ? utc.ToLocalTime()
                : TimeZoneInfo.ConvertTimeFromUtc(utc, _timeZoneInfo);
        }

        internal static void SetupTimeZoneInfo(string id)
        {
            _timeZoneInfo = string.IsNullOrWhiteSpace(id) ? null : TimeZoneInfo.FindSystemTimeZoneById(id);
        }

        private static TimeZoneInfo _timeZoneInfo;
    }
}
