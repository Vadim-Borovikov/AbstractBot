namespace AbstractBot.Interfaces.Modules.Context;

public interface ILocalizationUserSaveData : IUserSaveData
{
    public string? LanguageCode { get; }
}