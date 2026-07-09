using AbstractBot.Interfaces.Operations;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface IWrongOperationProcessor
{
    Task ProcessUnclearOperationAsync(IUpdateSender sender, Message message, User? user);
    Task ProcessInsufficientAccessAsync(IUpdateSender sender, Message message, User user, IOperation operation);
    Task ProcessExpiredAccessAsync(IUpdateSender sender, Message message, User user, IOperation operation);
}