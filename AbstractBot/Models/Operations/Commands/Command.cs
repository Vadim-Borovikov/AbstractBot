using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models.Config;
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

    public override MenuOperationInfo? GetHelpOperationInfoFor(long userId)
    {
        return BotCommandExtended.GetHelpOperationInfoFor(userId);
    }

    protected override bool IsInvokingBy(Message message, User? from)
    {
        return from is not null && BotCommandExtended.TryGetParameters(message) is not null;
    }
}