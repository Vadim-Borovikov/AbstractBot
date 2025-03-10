using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities.Save;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context;

[PublicAPI]
public class BotState<TBotStateData, TUserState, TUserStateData> : IBotState<TBotStateData, TUserState, TUserStateData>
    where TUserState : IStateful<TUserStateData>, new()
    where TBotStateData : IBotStateData<TUserStateData>, new()
{
    public Dictionary<long, TUserState> UserStates { get; init; }

    public BotState(Dictionary<long, TUserState> userStates) => UserStates = userStates;

    public virtual TBotStateData Save() => new() { UsersData = UserStates.Convert(d => d.Save()) };

    public virtual void LoadFrom(TBotStateData? data)
    {
        UserStates.Clear();
        if (data is null)
        {
            return;
        }
        foreach (long id in data.UsersData.Keys)
        {
            UserStates[id] = new TUserState();
            UserStates[id].LoadFrom(data.UsersData[id]);
        }
    }
}