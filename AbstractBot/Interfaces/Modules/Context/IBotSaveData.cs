using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface IBotSaveData<TUserSaveData>
{
    Dictionary<long, TUserSaveData> UsersData { get; set; }
}