using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces;

[PublicAPI]
public interface IConnection : IService
{
    string Host { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task ReconnectAsync(CancellationToken cancellationToken);
}