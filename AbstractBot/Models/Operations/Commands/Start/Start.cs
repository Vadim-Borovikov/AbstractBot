using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands.Start;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands.Start;

[PublicAPI]
public sealed class Start : Command, IStartCommand
{
    public Start(IAccesses accesses, IUpdateSender updateSender, ICommands commands,
        ITextsProvider<ITexts> textsProvider, string selfUsername, IGreeter greeter,
        IUserRegistrator? userRegistrator = null)
        : base(accesses, updateSender, "start", textsProvider, selfUsername)
    {
        _startCommon = new StartCommon(commands, userRegistrator);
        _greeter = greeter;
    }

    protected override async Task ExecuteAsync(Message message, User from)
    {
        await _startCommon.ExecuteAsync(from);
        await _greeter.Greet(message, from);
    }

    private readonly StartCommon _startCommon;
    private readonly IGreeter _greeter;
}