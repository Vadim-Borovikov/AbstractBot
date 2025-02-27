using AbstractBot.Models.MessageTemplates;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Config;

[PublicAPI]
public interface ITexts
{
    MessageTemplateText StartFormat { get; }

    MessageTemplateText? HelpFormat { get; }

    MessageTemplateText StatusMessageStartFormat { get; }

    MessageTemplateText StatusMessageEndFormat { get; }

    MessageTemplateText CommandDescriptionFormat { get; }

    string? TryGetCommandDescription(string command);

    string GetCommandDescription(string command)
    {
        return TryGetCommandDescription(command).Denull($"No description for /{command}!");
    }
}