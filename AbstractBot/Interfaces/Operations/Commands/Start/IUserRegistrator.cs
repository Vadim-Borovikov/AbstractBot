using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Operations.Commands.Start;

public interface IUserRegistrator
{
    bool RegistrerUser(User user);
}