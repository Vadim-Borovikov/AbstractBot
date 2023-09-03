using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace AbstractBot.Configs;

[PublicAPI]
public class Texts
{
    [Required]
    [MinLength(1)]
    public string StartCommandDescription { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string HelpCommandDescription { get; init; } = null!;

    public List<string?> AboutLines { get; init; } = null!;
    public List<string?>? StartPostfixLines { get; init; }
    public List<string?>? HelpPrefixLines { get; init; }
}