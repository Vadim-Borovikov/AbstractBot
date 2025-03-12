using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands.Start;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands.Start;

internal sealed class StartCommon
{
    public StartCommon(ICommands commands, IUserRegistrator? userRegistrator = null)
    {
        _commands = commands;
        _userRegistrator = userRegistrator;
    }

    public Task ExecuteAsync(User from)
    {
        _userRegistrator?.RegistrerUser(from);
        return _commands.UpdateFor(from.Id);
    }

    private readonly ICommands _commands;
    private readonly IUserRegistrator? _userRegistrator;
}