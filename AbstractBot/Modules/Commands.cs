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
        ITextsProvider<ITexts> textsProvider, IEnumerable<long>? additionalUsers = null)
    {
        _client = client;
        _accesses = accesses;
        _updateReceiver = updateReceiver;
        _textsProvider = textsProvider;

        _userIds = additionalUsers is null ? accesses.Ids.Distinct() : accesses.Ids.Concat(additionalUsers).Distinct();
    }

    public Task UpdateFor(long userId, CancellationToken cancellationToken = default)
    {
        ITexts texts = _textsProvider.GetTextsFor(userId);
        IEnumerable<BotCommand> commands = GetMenuCommands(_accesses.GetAccess(userId), texts);
        return _client.SetMyCommands(commands, BotCommandScope.Chat(userId), cancellationToken: cancellationToken);
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

        foreach (long id in _userIds)
        {
            await UpdateFor(id, cancellationToken);
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
    private readonly IEnumerable<long> _userIds;
}