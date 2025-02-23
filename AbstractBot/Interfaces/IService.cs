using System.Threading;
using System.Threading.Tasks;

namespace AbstractBot.Interfaces;

public interface IService
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}