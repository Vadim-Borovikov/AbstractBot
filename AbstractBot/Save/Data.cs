using JetBrains.Annotations;
using System.Collections.Generic;

namespace AbstractBot.Save;

[PublicAPI]
public class Data
{
    public Dictionary<long, Context> Contexts { get; init; } = new();

    public T? GetContext<T>(long key) where T : Context
    {
        return Contexts.ContainsKey(key) ? Contexts[key] as T : null;
    }
}