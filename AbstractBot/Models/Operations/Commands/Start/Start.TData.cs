using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Interfaces.Operations.Commands.Start;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands.Start;

[PublicAPI]
public sealed class Start<TData> : Command<TData>, IStartCommand
    where TData : ICommandData<TData>
{
    public Start(IAccesses accesses, IUpdateSender updateSender, ICommands commands,
        ITextsProvider<ITexts> textsProvider, string selfUsername, IGreeter<TData> greeter,
        IUserRegistrator? userRegistrator = null)
        : base(accesses, updateSender, "start", textsProvider, selfUsername)
    {
        _startCommon = new StartCommon(commands, userRegistrator);
        _greeter = greeter;
    }

    protected override async Task ExecuteAsync(TData data, Message message, User from)
    {
        await _startCommon.ExecuteAsync(from);
        await _greeter.GreetAsync(message, from, data);
    }

    protected override async Task ExecuteAsync(Message message, User from)
    {
        await _startCommon.ExecuteAsync(from);

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (_greeter is IGreeter s)
        {
            await s.GreetAsync(message, from);
        }
    }

    private readonly StartCommon _startCommon;
    private readonly IGreeter<TData> _greeter;
}