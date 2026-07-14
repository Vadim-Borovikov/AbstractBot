using AbstractBot.Models.Config;
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

    MenuOperationInfo? GetMenuOperationInfo(string command);
    string GetMenuDescription(string command)
    {
        return GetMenuOperationInfo(command)?.Description ?? $"No description for /{command}!";
    }
}