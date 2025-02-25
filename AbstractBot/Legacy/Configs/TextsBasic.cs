using System.ComponentModel.DataAnnotations;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;

namespace AbstractBot.Legacy.Configs;

[PublicAPI]
public class TextsBasic
{
    [Required]
    [MinLength(1)]
    public string StartCommandDescription { get; init; } = null!;

    [Required]
    [MinLength(1)]
    public string HelpCommandDescription { get; init; } = null!;

    [Required]
    public MessageTemplateText StartFormat { get; init; } = null!;

    public MessageTemplateText? HelpFormat { get; init; }

    [Required]
    public MessageTemplateText StatusMessageStartFormat { get; init; } = null!;

    [Required]
    public MessageTemplateText StatusMessageEndFormat { get; init; } = null!;

    [Required]
    public MessageTemplateText CommandDescriptionFormat { get; init; } = null!;
}