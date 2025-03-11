using AbstractBot.Interfaces.Modules.Context.Localization;
using AbstractBot.Interfaces.Operations.Commands.Start;
using JetBrains.Annotations;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace AbstractBot.Modules.Context.Localization;

[PublicAPI]
public class LocalizationUserRegistrator<TLocalizationUserState, TLocalizationUserStateData> : IUserRegistrator
    where TLocalizationUserState : class, ILocalizationUserState<TLocalizationUserStateData>, new()
    where TLocalizationUserStateData : ILocalizationUserStateData
{
    public LocalizationUserRegistrator(Dictionary<long, TLocalizationUserState> userStates)
    {
        _userStates = userStates;
    }

    public virtual void RegistrerUser(User user)
    {
        if (_userStates.ContainsKey(user.Id))
        {
            return;
        }

        _userStates[user.Id] = new TLocalizationUserState
        {
            LanguageCode = user.LanguageCode
        };
    }

    private readonly Dictionary<long, TLocalizationUserState> _userStates;
}