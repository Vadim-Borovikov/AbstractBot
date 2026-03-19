using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Operations.Commands.Start;

[PublicAPI]
public interface IGreeter
{
    Task GreetAsync(Message message, User from);
}