using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbstractBot.Models.Config;

[PublicAPI]
public class Texts : ITexts
{
    [Required]
    [MinLength(1)]
    public Dictionary<string, string> MenuDescriptions { get; init; } = null!;

    [Required]
    public TextContent StartFormat { get; init; } = null!;

    public TextContent? HelpFormat { get; init; }

    [Required]
    public TextContent StatusMessageStartFormat { get; init; } = null!;

    [Required]
    public TextContent StatusMessageEndFormat { get; init; } = null!;

    [Required]
    public TextContent CommandDescriptionFormat { get; init; } = null!;

    public string? TryGetMenuDescription(string command) => MenuDescriptions.GetValueOrDefault(command);
}