using System.Collections.Generic;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Interfaces.Modules.Context.Localization;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules.TextProviders;

[PublicAPI]
public class Localization<TTexts, TBotSaveData, TUserFinalData, TUserSaveData> : ITextsProvider<TTexts>
    where TTexts : ITexts
    where TUserSaveData : class, ILocalizationUserSaveData
    where TUserFinalData : ILocalizationUserFinalData<TUserSaveData>
    where TBotSaveData : class
{
    public readonly Dictionary<string, TTexts> AllTexts;

    public readonly string DefaultLanguageCode;

    public Localization(Dictionary<string, TTexts> allTexts, string defaultLanguageCode,
        IBotFinalData<TBotSaveData, TUserFinalData, TUserSaveData> finalData)
    {
        AllTexts = allTexts;
        DefaultLanguageCode = defaultLanguageCode;
        _finalData = finalData;
    }

    public TTexts GetTextsFor(User user)
    {
        string languageCode = _finalData.UsersData.GetValueOrDefault(user.Id)?.LanguageCode
            ?? user.LanguageCode
            ?? DefaultLanguageCode;

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

    private readonly IBotFinalData<TBotSaveData, TUserFinalData, TUserSaveData> _finalData;
}