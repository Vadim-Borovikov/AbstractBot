using GryphonUtilities.Save;

namespace AbstractBot.Interfaces.Modules.Context;

public interface ILocalizationUserFinalData<TUserSaveData> : IFinalData<TUserSaveData>
    where TUserSaveData : class, ILocalizationUserSaveData
{
    public string? LanguageCode { get; }
}