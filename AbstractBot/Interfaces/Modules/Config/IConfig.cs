using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Config;

[PublicAPI]
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

    string? Host { get; }

    Dictionary<long, int> Accesses { get; }

    string SavePath { get; }

    ITexts Texts { get; }
}