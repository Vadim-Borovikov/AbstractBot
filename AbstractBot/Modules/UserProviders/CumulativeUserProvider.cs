using System.Collections.Generic;
using System.Linq;
using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules.UserProviders;

[PublicAPI]
public class CumulativeUserProvider : IUserProvider
{
    public IEnumerable<User> GetUsers() => _providers.SelectMany(p => p.GetUsers()).DistinctBy(u => u.Id);

    public CumulativeUserProvider(params IUserProvider[] providers) => _providers = providers;

    private readonly IUserProvider[] _providers;
}