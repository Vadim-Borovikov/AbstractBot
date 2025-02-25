using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Operations.Commands;

[PublicAPI]
public interface ICommandData<out TData>
    where TData : class, ICommandData<TData>
{
    static abstract TData? From(Message message, User from, string[] parameters);
}