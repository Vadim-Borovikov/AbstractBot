using GryphonUtilities.Save;

namespace AbstractBot.Interfaces.Modules.Context.Localization;

public interface ILocalizationUserState<TLocalizationUserStateData> : IStatefulReloadable<TLocalizationUserStateData>
    where TLocalizationUserStateData : ILocalizationUserStateData
{
    public string? LanguageCode { get; set; }
}