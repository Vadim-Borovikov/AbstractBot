using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models;
using AbstractBot.Models.Operations.Commands;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AbstractBot.Modules;

[PublicAPI]
public class Commands : ICommands
{
    public Commands(TelegramBotClient client, IAccesses accesses, IUpdateReceiver updateReceiver,
        ITextsProvider<ITexts> textsProvider, IUserProvider users)
    {
        _client = client;
        _accesses = accesses;
        _updateReceiver = updateReceiver;
        _textsProvider = textsProvider;
        _users = users;
    }

    public Task UpdateFor(User user, CancellationToken cancellationToken = default)
    {
        ITexts texts = _textsProvider.GetTextsFor(user);
        IEnumerable<BotCommand> commands = GetMenuCommands(_accesses.GetAccess(user.Id), texts);
        return _client.SetMyCommands(commands, BotCommandScope.Chat(user.Id), cancellationToken: cancellationToken);
    }

    public async Task UpdateForAll(CancellationToken cancellationToken = default)
    {
        await _client.DeleteMyCommands(cancellationToken: cancellationToken);
        await _client.DeleteMyCommands(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await _client.DeleteMyCommands(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        ITexts defaultTexts = _textsProvider.GetDefaultTexts();
        await _client.SetMyCommands(GetMenuCommands(AccessData.Default, defaultTexts),
            BotCommandScope.AllPrivateChats(), cancellationToken: cancellationToken);

        foreach (User user in _users.GetUsers())
        {
            await UpdateFor(user, cancellationToken);
        }
    }

    private IEnumerable<BotCommand> GetMenuCommands(AccessData accessLevel, ITexts texts)
    {
        IEnumerable<BotCommandExtended> commands =
            _updateReceiver.Operations
                           .OfType<ICommand>()
                           .Where(c => c.BotCommandExtended.ShowInMenu
                                       && accessLevel.IsSufficientAgainst(c.AccessRequired))
                           .Select(ca => ca.BotCommandExtended);

        foreach (BotCommandExtended c in commands)
        {
            string? description = texts.TryGetMenuDescription(c.Command);
            if (!string.IsNullOrWhiteSpace(description))
            {
                yield return new BotCommand(c.Command, description);
            }
        }
    }

    private readonly TelegramBotClient _client;
    private readonly IAccesses _accesses;
    private readonly IUpdateReceiver _updateReceiver;
    private readonly ITextsProvider<ITexts> _textsProvider;
    private readonly IUserProvider _users;
}