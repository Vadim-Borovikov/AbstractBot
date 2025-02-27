using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ICommands
{
    Task UpdateFor(User user, CancellationToken cancellationToken = default);

    Task UpdateForAll(CancellationToken cancellationToken = default);
}