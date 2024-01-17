using System.ComponentModel.DataAnnotations;
using AbstractBot.Configs.MessageTemplates;
using JetBrains.Annotations;

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
    public MessageTemplateText StartFormat { get; init; } = null!;

    public MessageTemplateText? HelpFormat { get; init; }

    [Required]
    public MessageTemplateText StatusMessageStartFormat { get; init; } = null!;

    [Required]
    public MessageTemplateText StatusMessageEndFormat { get; init; } = null!;

    [Required]
    public MessageTemplateText CommandDescriptionFormat { get; init; } = null!;
}