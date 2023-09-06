using JetBrains.Annotations;
using System.Collections.Generic;

namespace AbstractBot.Save;

[PublicAPI]
public class Data
{
    public Dictionary<long, Context.Context> Contexts { get; init; } = new();
}
