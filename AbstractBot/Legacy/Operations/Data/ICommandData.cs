using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations.Data;

[PublicAPI]
public interface ICommandData<out T> where T : ICommandData<T>
{
    static abstract T? From(Message message, User sender, string[] parameters);
}