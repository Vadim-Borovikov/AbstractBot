using JetBrains.Annotations;

namespace AbstractBot.Operations.Infos;

[PublicAPI]
public interface ICommandInfo<out T> where T : ICommandInfo<T>
{
    static abstract T? From(string[] parameters);
}