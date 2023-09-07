using AbstractBot.Bots;
using AbstractBot.Operations.Infos;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class CommandSimple : Command<InfoCommand>
{
    protected CommandSimple(BotBasic bot, string command, string description)
        : base(bot, command, description)
    { }

    protected override bool IsInvokingByPayload(Message message, User sender, string payload, out InfoCommand info)
    {
        info = new InfoCommand(payload);
        return payload == "";
    }
}