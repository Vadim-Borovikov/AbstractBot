using System.Collections.Generic;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Modules.Context.Localization;
using JetBrains.Annotations;

namespace AbstractBot.Modules.TextProviders;

[PublicAPI]
public class Localization<TTexts, TUserState, TUserStateData> : ITextsProvider<TTexts>
    where TTexts : ITexts
    where TUserState : ILocalizationUserState<TUserStateData>
    where TUserStateData : ILocalizationUserStateData
{
    public readonly Dictionary<string, TTexts> AllTexts;

    public readonly string DefaultLanguageCode;

    public Localization(Dictionary<string, TTexts> allTexts, string defaultLanguageCode,
        Dictionary<long, TUserState> userStates)
    {
        AllTexts = allTexts;
        DefaultLanguageCode = defaultLanguageCode;
        _userStates = userStates;
    }

    public TTexts GetTextsFor(long userId)
    {
        string languageCode = _userStates.GetValueOrDefault(userId)?.LanguageCode ?? DefaultLanguageCode;

        return GetTexts(languageCode);
    }

    public TTexts GetDefaultTexts() => GetTexts();

    public TTexts GetTexts(string? languageCode = null)
    {
        languageCode = !string.IsNullOrWhiteSpace(languageCode) && AllTexts.ContainsKey(languageCode)
            ? languageCode
            : DefaultLanguageCode;

        return AllTexts[languageCode];
    }

    private readonly Dictionary<long, TUserState> _userStates;
}