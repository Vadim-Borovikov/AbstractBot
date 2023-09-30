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
    public MessageTemplate Start { get; init; } = null!;

    public MessageTemplate? HelpFormat { get; init; }

    [Required]
    public MessageTemplate StatusMessageStartFormat { get; init; } = null!;

    [Required]
    public MessageTemplate StatusMessageEndFormat { get; init; } = null!;
}