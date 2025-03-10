using System.Collections.Generic;
using AbstractBot.Modules.Context;
using AbstractBot.Modules.Context.Localization;

namespace AbstractBot.Example;

internal sealed class ExampleBotState : BotState<ExampleStateData, ExampleUserState, LocalizationUserStateData>
{
    public ExampleBotState(Dictionary<long, ExampleUserState> userStates) : base(userStates) { }
}