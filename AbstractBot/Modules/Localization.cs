using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules;

[PublicAPI]
public class Localization<TTexts>
    where TTexts : ITexts
{
    public readonly Dictionary<string, TTexts> AllTexts;

    public readonly string DefaultLanguageCode;

    public Localization(Dictionary<string, TTexts> allTexts, string defaultLanguageCode)
    {
        AllTexts = allTexts;
        DefaultLanguageCode = defaultLanguageCode;
    }

    public TTexts GetTextFor(User user) => GetTexts(user.LanguageCode);

    public TTexts GetTexts(string? languageCode = null)
    {
        languageCode = !string.IsNullOrWhiteSpace(languageCode) && AllTexts.ContainsKey(languageCode)
            ? languageCode
            : DefaultLanguageCode;

        return AllTexts[languageCode];
    }
}