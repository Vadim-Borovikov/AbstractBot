using AbstractBot.Interfaces.Modules.Context.Localization;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context.Localization;

[PublicAPI]
public class LocalizationUserSaveData : ILocalizationUserSaveData
{
    [UsedImplicitly]
    public string? LanguageCode { get; set; }
}