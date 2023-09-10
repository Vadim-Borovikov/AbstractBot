using JetBrains.Annotations;

namespace AbstractBot.Operations.Infos;

[PublicAPI]
public class CommandInfoSimple : ICommandInfo<CommandInfoSimple>
{
    public static CommandInfoSimple? From(string[] parameters) => null;
}