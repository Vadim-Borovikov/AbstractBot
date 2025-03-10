using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Context;

public interface IBotStateData<TUserStateData>
{
    Dictionary<long, TUserStateData> UsersData { get; set; }
}