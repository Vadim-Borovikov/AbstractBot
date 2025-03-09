using GryphonUtilities.Save;
using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface ILocalizationBotFinalData<TLocalizationBotSaveData, TLocalizationUserFinalData, TLocalizationUserSaveData>
    : IFinalData<TLocalizationBotSaveData>
    where TLocalizationUserSaveData : class, ILocalizationUserSaveData
    where TLocalizationUserFinalData : ILocalizationUserFinalData<TLocalizationUserSaveData>
    where TLocalizationBotSaveData : class
{
    public Dictionary<long, TLocalizationUserFinalData> UsersData { get; }
}