using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models;
using AbstractBot.Models.Config;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Utilities;
using GryphonUtilities.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace AbstractBot.Modules;

[PublicAPI]
public class Commands : ICommands
{
    public Commands(TelegramBotClient client, IAccesses accesses, IUpdateReceiver updateReceiver,
        ITextsProvider<ITexts> textsProvider, Logger logger, IEnumerable<long>? additionalUsers = null)
    {
        _client = client;
        _accesses = accesses;
        _updateReceiver = updateReceiver;
        _textsProvider = textsProvider;
        _logger = logger;

        _userIds = additionalUsers is null ? accesses.Ids.Distinct() : accesses.Ids.Concat(additionalUsers).Distinct();
    }

    public async Task UpdateFor(long userId, CancellationToken cancellationToken = default)
    {
        ITexts texts = _textsProvider.GetTextsFor(userId);
        IEnumerable<BotCommand> commands = GetMenuCommands(_accesses.GetAccess(userId), texts);
        try
        {
            await _client.SetMyCommands(commands, BotCommandScope.Chat(userId), cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex) when (ErrorHelper.IsChatNotFoundError(ex))
        {
            _logger.Errors.Log($"Exception caught: {ex.Message}", true);
        }
    }

    public async Task UpdateForUsers(CancellationToken cancellationToken = default)
    {
        foreach (long id in _userIds)
        {
            await UpdateFor(id, cancellationToken);
        }
    }

    public async Task UpdateForAll(CancellationToken cancellationToken = default)
    {
        await ResetForAll(cancellationToken);
        await UpdateForUsers(cancellationToken);
    }

    public async Task ResetForAll(CancellationToken cancellationToken = default)
    {
        await _client.DeleteMyCommands(cancellationToken: cancellationToken);
        await _client.DeleteMyCommands(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await _client.DeleteMyCommands(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        ITexts defaultTexts = _textsProvider.GetDefaultTexts();
        await _client.SetMyCommands(GetMenuCommands(AccessData.Default, defaultTexts),
            BotCommandScope.AllPrivateChats(), cancellationToken: cancellationToken);
    }

    private IEnumerable<BotCommand> GetMenuCommands(AccessData accessLevel, ITexts texts)
    {
        Dictionary<string, MenuOperationInfo> infos = new();

        foreach (ICommand command in _updateReceiver.Operations
                                                    .OfType<ICommand>()
                                                    .Where(c => c.BotCommandExtended.ShowInMenu
                                                                && accessLevel.IsSufficientAgainst(c.AccessRequired)))
        {
            MenuOperationInfo? info = texts.GetMenuOperationInfo(command.BotCommandExtended.Command);
            if (info is not null)
            {
                infos[command.BotCommandExtended.Command] = info.Value;
            }
        }

        return infos.OrderBy(p => p.Value.Index)
                    .Select(p => new BotCommand(p.Key, p.Value.Description));
    }

    private readonly TelegramBotClient _client;
    private readonly IAccesses _accesses;
    private readonly IUpdateReceiver _updateReceiver;
    private readonly ITextsProvider<ITexts> _textsProvider;
    private readonly Logger _logger;
    private readonly IEnumerable<long> _userIds;
}