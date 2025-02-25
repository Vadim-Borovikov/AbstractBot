using AbstractBot.Interfaces.Operations.Commands;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations.Data;

[PublicAPI]
public class CommandDataSimple : ICommandData<CommandDataSimple>
{
    public static CommandDataSimple? From(Message message, User sender, string[] parameters) => null;
}