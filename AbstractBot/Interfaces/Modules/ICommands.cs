using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ICommands
{
    Task UpdateFor(long userId, CancellationToken cancellationToken = default);

    Task UpdateForAll(CancellationToken cancellationToken = default);
}