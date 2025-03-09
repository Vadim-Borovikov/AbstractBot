using GryphonUtilities.Save;
using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface IBotFinalData<TBotSaveData, TUserFinalData, TUserSaveData> : IFinalData<TBotSaveData>
    where TUserSaveData : class
    where TUserFinalData : IFinalData<TUserSaveData>
    where TBotSaveData : class
{
    public Dictionary<long, TUserFinalData> UsersData { get; }
}