using JetBrains.Annotations;

namespace AbstractBot.Operations.Data;

[PublicAPI]
public interface ICommandData<out T> where T : ICommandData<T>
{
    static abstract T? From(string[] parameters);
}