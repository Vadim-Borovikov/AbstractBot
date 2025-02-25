using System.Collections.Generic;
using System.Linq;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public abstract class Command<TData> : Operation<TData>, ICommand
    where TData : class, ICommandData<TData>
{
    public BotCommandExtended BotCommandExtended { get; }

    protected Command(IAccesses accesses, IUpdateSender updateSender, string command, string menuDescription,
        string selfUsername, bool showInMenu = true)
        : base(accesses, updateSender)
    {
        BotCommandExtended = new BotCommandExtended(command, menuDescription, selfUsername, showInMenu);
        HelpDescription = new MessageTemplateText(menuDescription);
    }

    protected override bool IsInvokingBy(Message message, User from, out TData? data)
    {
        data = null;
        IEnumerable<string>? parameters = BotCommandExtended.TryGetParameters(message);
        if (parameters is not null)
        {
            data = TData.From(message, from, parameters.ToArray());
        }
        return data is not null;
    }
}