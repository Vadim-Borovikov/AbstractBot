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

    [Required]
    public MessageText Start { get; init; } = null!;

    public MessageText? HelpFormat { get; init; }

    [Required]
    public MessageText StatusMessageStartFormat { get; init; } = null!;

    [Required]
    public MessageText StatusMessageEndFormat { get; init; } = null!;
}