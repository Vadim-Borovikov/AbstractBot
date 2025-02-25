using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ICommands
{
    Task UpdateCommandsFor(long userId, CancellationToken cancellationToken = default);

    Task UpdateCommands(CancellationToken cancellationToken = default);
}