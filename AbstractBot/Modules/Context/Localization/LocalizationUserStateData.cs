using AbstractBot.Interfaces.Modules.Context.Localization;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context.Localization;

[PublicAPI]
public class LocalizationUserStateData : ILocalizationUserStateData
{
    [UsedImplicitly]
    public string? LanguageCode { get; init; }
}