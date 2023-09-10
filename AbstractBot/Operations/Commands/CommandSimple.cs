using AbstractBot.Bots;
using AbstractBot.Operations.Infos;
using JetBrains.Annotations;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class CommandSimple : Command<CommandInfoSimple>
{
    protected CommandSimple(BotBasic bot, string command, string description)
        : base(bot, command, description)
    { }
}