using System.Collections.Generic;
using System.Linq;
using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules.UserProviders;

[PublicAPI]
public class AccessBasedUserProvider : IUserProvider
{
    public IEnumerable<User> GetUsers() => _accesses.Ids.Select(id => new User { Id = id });

    public AccessBasedUserProvider(IAccesses accesses) => _accesses = accesses;

    private readonly IAccesses _accesses;
}