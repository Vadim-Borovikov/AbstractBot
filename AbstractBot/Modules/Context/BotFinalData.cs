using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities.Save;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context;

[PublicAPI]
public class BotFinalData<TBotSaveData, TUserFinalData, TUserSaveData>
    : IBotFinalData<TBotSaveData, TUserFinalData, TUserSaveData>
    where TUserSaveData : class
    where TUserFinalData : IFinalData<TUserSaveData>, new()
    where TBotSaveData : class, IBotSaveData<TUserSaveData>, new()
{
    public Dictionary<long, TUserFinalData> UsersData { get; set; } = new();

    public TBotSaveData Save() => new() { UsersData = UsersData.Convert(d => d.Save()) };

    public void LoadFrom(TBotSaveData? data)
    {
        UsersData.Clear();
        if (data is null)
        {
            return;
        }
        foreach (long id in data.UsersData.Keys)
        {
            UsersData[id] = new TUserFinalData();
            UsersData[id].LoadFrom(data.UsersData[id]);
        }
    }
}