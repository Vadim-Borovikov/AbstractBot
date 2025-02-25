using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
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
    public Commands(TelegramBotClient client, IAccesses accesses, IUpdateReceiver updateReceiver)
    {
        _client = client;
        _accesses = accesses;
        _updateReceiver = updateReceiver;
    }

    public Task UpdateCommandsFor(long userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<BotCommandExtended> commands = GetMenuCommands(_accesses.GetAccess(userId));
        return _client.SetMyCommands(commands, BotCommandScope.Chat(userId), cancellationToken: cancellationToken);
    }

    public async Task UpdateCommands(CancellationToken cancellationToken = default)
    {
        await _client.DeleteMyCommands(cancellationToken: cancellationToken);
        await _client.DeleteMyCommands(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await _client.DeleteMyCommands(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        await _client.SetMyCommands(GetMenuCommands(AccessData.Default), BotCommandScope.AllPrivateChats(),
            cancellationToken: cancellationToken);

        foreach (long userId in _accesses.Ids)
        {
            await _client.SetMyCommands(GetMenuCommands(_accesses.GetAccess(userId)), BotCommandScope.Chat(userId),
                cancellationToken: cancellationToken);
        }
    }

    private IEnumerable<BotCommandExtended> GetMenuCommands(AccessData accessLevel)
    {
        return _updateReceiver.Operations
                              .OfType<ICommand>()
                              .Where(c => c.BotCommandExtended.ShowInMenu
                                          && accessLevel.IsSufficientAgainst(c.AccessRequired))
                              .Select(ca => ca.BotCommandExtended);
    }

    private readonly TelegramBotClient _client;
    private readonly IAccesses _accesses;
    private readonly IUpdateReceiver _updateReceiver;
}