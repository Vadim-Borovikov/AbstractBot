using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GoogleSheetsManager;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace AbstractBot.Configs;

[PublicAPI]
public class ConfigGoogleSheets : Config, IConfigGoogleSheets
{
    public Dictionary<string, string>? GoogleCredential { get; init; }
    public string? GoogleCredentialJson { get; init; }

    public Dictionary<string, string>? Credential => GoogleCredential;
    public string? CredentialJson => GoogleCredentialJson;

    [Required]
    [MinLength(1)]
    public string ApplicationName { get; init; } = null!;

    public string TimeZoneId => SystemTimeZoneId;
}