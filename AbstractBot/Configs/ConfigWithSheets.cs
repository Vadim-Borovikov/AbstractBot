using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GoogleSheetsManager;
using JetBrains.Annotations;

namespace AbstractBot.Configs;

[PublicAPI]
public class ConfigWithSheets<T> : Config<T>, IConfigGoogleSheets
    where T : Texts
{
    public Dictionary<string, string> Credential => GoogleCredential;

    [Required]
    public Dictionary<string, string> GoogleCredential { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string ApplicationName { get; init; } = null!;

    public string TimeZoneId => SystemTimeZoneId;
}