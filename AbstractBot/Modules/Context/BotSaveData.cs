using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context;

[PublicAPI]
public class BotSaveData<TUserSaveData> : IBotSaveData<TUserSaveData>
{
    [UsedImplicitly]
    public Dictionary<long, TUserSaveData> UsersData { get; set; } = new();
}