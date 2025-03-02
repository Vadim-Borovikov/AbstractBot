using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using JetBrains.Annotations;

namespace AbstractBot.Models.Context;

[PublicAPI]
public class ContextSaveData<TUserSaveData> : IBotSaveData<TUserSaveData>
    where TUserSaveData : class, IUserSaveData, new()
{
    [UsedImplicitly]
    public Dictionary<long, TUserSaveData> UsersData { get; set; } = new();
}