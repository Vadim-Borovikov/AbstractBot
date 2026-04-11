using GryphonUtilities.Save;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Context;

[PublicAPI]
public interface IBotState<TBotSaveData, TUserState, TUserStateData> : IStatefulReloadable<TBotSaveData>
    where TUserState : IStatefulReloadable<TUserStateData>
{
    public Dictionary<long, TUserState> UserStates { get; }
}