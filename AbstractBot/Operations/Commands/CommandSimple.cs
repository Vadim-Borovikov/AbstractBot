using AbstractBot.Bots;
using AbstractBot.Operations.Data;
using JetBrains.Annotations;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class CommandSimple : Command<CommandDataSimple>
{
    protected CommandSimple(BotBasic bot, string command, string description)
        : base(bot, command, description)
    { }
}