using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Operations.Commands.Start;

[PublicAPI]
public interface IGreeter<in TData>
    where TData : class, ICommandData<TData>
{
    Task Greet(Message message, User from, TData data);
}