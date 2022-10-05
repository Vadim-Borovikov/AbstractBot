using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace AbstractBot;

[PublicAPI]
public class ConfigGoogleSheets : Config
{
    public Dictionary<string, string>? GoogleCredential { get; init; }
    public string? GoogleCredentialJson { get; init; }

    [Required]
    [MinLength(1)]
    public string ApplicationName { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string GoogleSheetId { get; init; } = null!;
}
