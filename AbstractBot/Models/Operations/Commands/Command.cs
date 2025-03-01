using System.Collections.Generic;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public abstract class Command : Operation, ICommand
{
    public BotCommandExtended BotCommandExtended { get; }

    protected Command(IAccesses accesses, IUpdateSender updateSender, string command, ITexts texts,
        string selfUsername, bool showInMenu = true)
        : base(accesses, updateSender)
    {
        string menuDescription = texts.GetCommandDescription(command);
        BotCommandExtended = new BotCommandExtended(command, menuDescription, selfUsername, showInMenu);
        HelpDescription = texts.CommandDescriptionFormat.Format(command, menuDescription);
    }

    protected override bool IsInvokingBy(Message message, User from)
    {
        IEnumerable<string>? parameters = BotCommandExtended.TryGetParameters(message);
        return parameters is not null;
    }
}