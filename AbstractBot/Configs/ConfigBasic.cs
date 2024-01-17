using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AbstractBot.Configs;

[PublicAPI]
public class ConfigBasic
{
    [Required]
    [MinLength(1)]
    public string Token { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string SystemTimeZoneId { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string SystemTimeZoneIdLogs { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string DontUnderstandStickerFileId { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string ForbiddenStickerFileId { get; init; } = null!;

    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double UpdatesPerSecondLimitPrivate { get; init; }

    [Required]
    [Range(double.Epsilon, double.MaxValue)]

    public double UpdatesPerMinuteLimitGroup { get; set; }
    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double UpdatesPerSecondLimitGlobal { get; set; }

    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double RestartPeriodHours { get; set; }

    public string? Host { get; init; }

    [Required]
    public Texts Texts { get; init; } = null!;

    [Required]
    public Dictionary<long, int> Accesses { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string SavePath { get; init; } = null!;

    public byte HelpCommandMenuOrder { get; init; } = 1;
}