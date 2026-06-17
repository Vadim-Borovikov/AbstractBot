using AbstractBot.Models.Config;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Config;

[PublicAPI]
public interface ITexts
{
    TextContent StartFormat { get; }

    TextContent? HelpFormat { get; }

    TextContent StatusMessageStartFormat { get; }

    TextContent StatusMessageEndFormat { get; }

    TextContent CommandDescriptionFormat { get; }

    string? TryGetMenuDescription(string command);

    string GetMenuDescription(string command)
    {
        return TryGetMenuDescription(command).Denull($"No description for /{command}!");
    }
}