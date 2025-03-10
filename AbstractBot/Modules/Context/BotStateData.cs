using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context;

[PublicAPI]
public class BotStateData<TUserStateData> : IBotStateData<TUserStateData>
{
    [UsedImplicitly]
    public Dictionary<long, TUserStateData> UsersData { get; set; } = new();
}