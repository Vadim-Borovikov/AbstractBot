using AbstractBot.Interfaces.Modules.Context.Localization;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context.Localization;

[PublicAPI]
public class LocalizationUserState<TLocalizationUserStateData> : ILocalizationUserState<TLocalizationUserStateData>
    where TLocalizationUserStateData : ILocalizationUserStateData, new()
{
    public string? LanguageCode { get; set; }
    public virtual TLocalizationUserStateData Save() => new() { LanguageCode = LanguageCode };
    public virtual void LoadFrom(TLocalizationUserStateData? data) => LanguageCode = data?.LanguageCode;
}