using GryphonUtilities.Save;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Context;

[PublicAPI]
public interface IBotState<TBotSaveData, TUserState, TUserStateData> : IStateful<TBotSaveData>
    where TUserState : IStateful<TUserStateData>
{
    public Dictionary<long, TUserState> UserStates { get; }
}