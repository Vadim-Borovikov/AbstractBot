using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;

namespace AbstractBot.Example;

internal sealed class ExampleFinalData
    : ILocalizationBotFinalData<ExampleSaveData, ExampleUserFinalData, ExampleUserSaveData>
{
    public Dictionary<long, ExampleUserFinalData> UsersData { get; set; } = new();

    public ExampleSaveData Save() => new() { UsersData = UsersData.Convert(d => d.Save()) };

    public void LoadFrom(ExampleSaveData? data)
    {
        UsersData.Clear();
        if (data is null)
        {
            return;
        }
        foreach (long id in data.UsersData.Keys)
        {
            UsersData[id] = new ExampleUserFinalData();
            UsersData[id].LoadFrom(data.UsersData[id]);
        }
    }
}