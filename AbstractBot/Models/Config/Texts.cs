using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;

namespace AbstractBot.Models.Config;

[PublicAPI]
public class Texts : ITexts
{
    [Required]
    [MinLength(1)]
    public Dictionary<string, string> MenuDescriptions { get; init; } = null!;

    [Required]
    public MessageTemplateText StartFormat { get; init; } = null!;

    public MessageTemplateText? HelpFormat { get; init; }

    [Required]
    public MessageTemplateText StatusMessageStartFormat { get; init; } = null!;

    [Required]
    public MessageTemplateText StatusMessageEndFormat { get; init; } = null!;

    [Required]
    public MessageTemplateText CommandDescriptionFormat { get; init; } = null!;

    public string? TryGetMenuDescription(string command) => MenuDescriptions.GetValueOrDefault(command);
}