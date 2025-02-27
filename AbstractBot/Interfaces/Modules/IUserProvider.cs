using System.Collections.Generic;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface IUserProvider
{
    IEnumerable<User> GetUsers();
}