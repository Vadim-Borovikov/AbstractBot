using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Config;

[PublicAPI]
public interface ITexts
{
    string StartCommandDescription { get; }

    string HelpCommandDescription { get; }

    MessageTemplateText StartFormat { get; }

    MessageTemplateText? HelpFormat { get; }

    MessageTemplateText StatusMessageStartFormat { get; }

    MessageTemplateText StatusMessageEndFormat { get; }

    MessageTemplateText CommandDescriptionFormat { get; }
}