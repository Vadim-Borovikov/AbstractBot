using System.Collections.Generic;

namespace AbstractBot.Interfaces.Modules.Config;

public interface ILocalizationConfig<TTexts> : IConfig
    where TTexts : ITexts
{
    Dictionary<string, TTexts> AllTexts { get; }

    string DefaultLanguageCode { get; }
}