using AbstractBot.Interfaces.Modules.Context.Localization;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context.Localization;

[PublicAPI]
public class LocalizationUserFinalData<TUserSaveData> : ILocalizationUserFinalData<TUserSaveData>
    where TUserSaveData : class, ILocalizationUserSaveData, new()
{
    public string? LanguageCode { get; set; }
    public TUserSaveData Save() => new() { LanguageCode = LanguageCode };
    public void LoadFrom(TUserSaveData? data) => LanguageCode = data?.LanguageCode;
}