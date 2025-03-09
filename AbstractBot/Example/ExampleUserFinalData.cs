using AbstractBot.Interfaces.Modules.Context;

namespace AbstractBot.Example;

public sealed class ExampleUserFinalData : ILocalizationUserFinalData<ExampleUserSaveData>
{
    public string? LanguageCode { get; set; }
    public ExampleUserSaveData Save() => new() { LanguageCode = LanguageCode };
    public void LoadFrom(ExampleUserSaveData? data) => LanguageCode = data?.LanguageCode;
}