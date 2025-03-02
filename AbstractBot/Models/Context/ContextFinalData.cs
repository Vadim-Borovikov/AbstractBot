using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;
using JetBrains.Annotations;

namespace AbstractBot.Models.Context;

[PublicAPI]
public abstract class ContextFinalData<TContextFinalData, TContextSaveData, TUserFinalData, TUserSaveData> :
    IBotFinalData<TContextFinalData, TContextSaveData, TUserFinalData, TUserSaveData>
    where TContextFinalData : class, IBotFinalData<TContextFinalData, TContextSaveData, TUserFinalData, TUserSaveData>, new()
    where TContextSaveData : class, IBotSaveData<TUserSaveData>, new()
    where TUserFinalData : class, IUserFinalData<TUserFinalData, TUserSaveData>, new()
    where TUserSaveData : class, IUserSaveData, new()
{
    public Dictionary<long, TUserFinalData> UsersData { get; set; } = new();

    public TContextSaveData Save() => new() { UsersData = UsersData.Convert(d => d.Save()) };

    public static TContextFinalData Load(TContextSaveData data)
    {
        return new TContextFinalData
        {
            UsersData = data.UsersData.Convert(TUserFinalData.Load)
        };
    }
}