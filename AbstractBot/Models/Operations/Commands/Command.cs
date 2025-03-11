using System.Collections.Generic;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public abstract class Command : Operation, ICommand
{
    public BotCommandExtended BotCommandExtended { get; }

    protected Command(IAccesses accesses, IUpdateSender updateSender, string command,
        ITextsProvider<ITexts> textsProvider, string selfUsername, bool showInMenu = true)
        : base(accesses, updateSender)
    {
        ITexts defaultTexts = textsProvider.GetDefaultTexts();
        string menuDescription = defaultTexts.GetMenuDescription(command);
        BotCommandExtended = new BotCommandExtended(command, menuDescription, selfUsername, textsProvider, showInMenu);
    }

    public override MessageTemplateText? GetHelpDescriptionFor(long userId)
    {
        return BotCommandExtended.GetHelpDescriptionFor(userId);
    }

    protected override bool IsInvokingBy(Message message, User from)
    {
        IEnumerable<string>? parameters = BotCommandExtended.TryGetParameters(message);
        return parameters is not null;
    }
}