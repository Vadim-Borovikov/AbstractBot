using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;

namespace AbstractBot.Example;

internal sealed class ExampleFinalData
    : IBotFinalData<ExampleFinalData, ExampleSaveData, ExampleUserFinalData, ExampleUserSaveData>
{
    public Dictionary<long, ExampleUserFinalData> UsersData { get; set; } = new();

    public ExampleSaveData Save() => new() { UsersData = UsersData.Convert(d => d.Save()) };

    public static ExampleFinalData Load(ExampleSaveData data)
    {
        return new ExampleFinalData
        {
            UsersData = data.UsersData.Convert(ExampleUserFinalData.Load)
        };
    }
}