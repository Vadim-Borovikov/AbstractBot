using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Example;

public sealed class ExampleSaveData
{
    [UsedImplicitly]
    public List<int>? List { get; set; }
}