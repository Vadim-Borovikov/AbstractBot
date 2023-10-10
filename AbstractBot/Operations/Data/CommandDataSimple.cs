using JetBrains.Annotations;

namespace AbstractBot.Operations.Data;

[PublicAPI]
public class CommandDataSimple : ICommandData<CommandDataSimple>
{
    public static CommandDataSimple? From(string[] parameters) => null;
}