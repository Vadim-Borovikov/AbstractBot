namespace AbstractBot.Interfaces.Modules.Context;

public interface ILocalizationUserFinalData<out TUserFinalData, TUserSaveData> : IUserFinalData<TUserFinalData, TUserSaveData>
    where TUserFinalData : class, ILocalizationUserFinalData<TUserFinalData, TUserSaveData>, new()
    where TUserSaveData : class, ILocalizationUserSaveData, new()
{
    public string? LanguageCode { get; }
} 