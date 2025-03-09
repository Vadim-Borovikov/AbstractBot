using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;

namespace AbstractBot.Models.Config;

[PublicAPI]
public class ConfigWithSheets : Config, ISheetsConfig
{
    [Required]
    public Dictionary<string, string> GoogleCredential { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string ApplicationName { get; init; } = null!;
}