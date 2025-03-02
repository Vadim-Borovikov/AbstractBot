using GryphonUtilities.Save;
using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface IBotFinalData<out TBotFinalData, TBotSaveData, TUserFinalData, TUserSaveData>
    : IFinalData<TBotFinalData, TBotSaveData>
    where TBotFinalData : class, IBotFinalData<TBotFinalData, TBotSaveData, TUserFinalData, TUserSaveData>, new()
    where TBotSaveData : class, IBotSaveData<TUserSaveData>, new()
    where TUserFinalData : class, IUserFinalData<TUserFinalData, TUserSaveData>, new()
    where TUserSaveData : class, IUserSaveData, new()
{
    public Dictionary<long, TUserFinalData> UsersData { get; set; }
}