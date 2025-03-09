using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using JetBrains.Annotations;

namespace AbstractBot.Example;

[PublicAPI]
public class ExampleSaveData : ILocalizationBotSaveData<ExampleUserSaveData>
{
    [UsedImplicitly]
    public Dictionary<long, ExampleUserSaveData> UsersData { get; set; } = new();
}