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
    where TData : class, ICommandData<TData>
{
    internal Start(IAccesses accesses, IUpdateSender updateSender, ICommands commands,
        ITextsProvider<ITexts> textsProvider, string selfUsername, IGreeter<TData> greeter,
        IUserRegistrator? userRegistrator = null)
        : base(accesses, updateSender, "start", textsProvider, selfUsername)
    {
        _commands = commands;
        _greeter = greeter;
        _userRegistrator = userRegistrator;
    }

    protected override async Task ExecuteAsync(TData data, Message message, User from)
    {
        _userRegistrator?.RegistrerUser(from);
        await _commands.UpdateFor(from.Id);
        await _greeter.Greet(message, from, data);
    }

    protected override async Task ExecuteAsync(Message message, User from)
    {
        _userRegistrator?.RegistrerUser(from);

        await _commands.UpdateFor(from.Id);

        // ReSharper disable once SuspiciousTypeConversion.Global
        if (_greeter is IGreeter s)
        {
            await s.Greet(message, from);
        }
    }

    private readonly ICommands _commands;
    private readonly IGreeter<TData> _greeter;
    private readonly IUserRegistrator? _userRegistrator;
}