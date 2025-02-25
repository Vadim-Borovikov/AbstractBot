using System.Collections.Generic;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public abstract class Command : Operation, ICommand
{
    public BotCommandExtended BotCommandExtended { get; }

    protected Command(IAccesses accesses, IUpdateSender updateSender, string command, string menuDescription,
        string selfUsername, bool showInMenu = true)
        : base(accesses, updateSender)
    {
        BotCommandExtended = new BotCommandExtended(command, menuDescription, selfUsername, showInMenu);
        HelpDescription = new MessageTemplateText(menuDescription);
    }

    protected override bool IsInvokingBy(Message message, User from)
    {
        IEnumerable<string>? parameters = BotCommandExtended.TryGetParameters(message);
        return parameters is not null;
    }
}