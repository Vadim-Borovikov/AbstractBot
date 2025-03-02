using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface IBotSaveData<TUserSaveData>
    where TUserSaveData : class, IUserSaveData, new()
{
    Dictionary<long, TUserSaveData> UsersData { get; set; }
}