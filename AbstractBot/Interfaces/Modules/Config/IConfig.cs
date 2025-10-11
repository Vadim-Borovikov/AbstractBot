using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Config;

public interface IConfig
{
    string Token { get; }

    string SystemTimeZoneId { get; }

    string SystemTimeZoneIdLogs { get; }

    string DontUnderstandStickerFileId { get; }

    string ForbiddenStickerFileId { get; }

    double UpdatesPerSecondLimitPrivate { get; }

    double UpdatesPerMinuteLimitGroup { get; }

    double UpdatesPerSecondLimitGlobal { get; }

    double RestartPeriodHours { get; }

    double TickIntervalSeconds { get; }

    byte MaxMessagesInBatch { get; }

    public long ReportsDefaultChatId { get; }

    string? Host { get; }

    Dictionary<long, int> Accesses { get; }
}