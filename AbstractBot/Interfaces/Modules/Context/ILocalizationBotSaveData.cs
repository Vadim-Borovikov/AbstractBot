using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface ILocalizationBotSaveData<TUserSaveData>
    where TUserSaveData : ILocalizationUserSaveData
{
    Dictionary<long, TUserSaveData> UsersData { get; }
}