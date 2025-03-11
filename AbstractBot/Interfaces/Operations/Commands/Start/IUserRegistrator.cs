using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Operations.Commands.Start;

public interface IUserRegistrator
{
    void RegistrerUser(User user);
}