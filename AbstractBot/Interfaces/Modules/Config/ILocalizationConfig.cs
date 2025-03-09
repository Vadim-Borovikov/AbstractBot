using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Config;

[PublicAPI]
public interface ILocalizationConfig<TTexts> : IConfig
    where TTexts : ITexts
{
    Dictionary<string, TTexts> AllTexts { get; }

    string DefaultLanguageCode { get; }
}