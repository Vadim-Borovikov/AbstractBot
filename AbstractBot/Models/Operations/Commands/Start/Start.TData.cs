using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Interfaces.Operations.Commands.Start;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands.Start;

[PublicAPI]
public sealed class Start<TData> : Command<TData>
    where TData : class, ICommandData<TData>
{
    internal Start(IAccesses accesses, IUpdateSender updateSender, ICommands commands, string description,
        string selfUsername, IGreeter<TData> greeter)
        : base(accesses, updateSender, "start", description, selfUsername)
    {
        _commands = commands;
        _greeter = greeter;
    }

    protected override async Task ExecuteAsync(TData data, Message message, User from)
    {
        await _commands.UpdateCommandsFor(from.Id);
        await _greeter.Greet(message, from, data);
    }

    protected override async Task ExecuteAsync(Message message, User from)
    {
        await _commands.UpdateCommandsFor(from.Id);

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (_greeter is IGreeter s)
        {
            await s.Greet(message, from);
        }
    }

    private readonly ICommands _commands;
    private readonly IGreeter<TData> _greeter;
}