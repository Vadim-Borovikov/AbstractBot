using AbstractBot.Interfaces.Modules.Context;

namespace AbstractBot.Example;

public sealed class ExampleUserFinalData : ILocalizationUserFinalData<ExampleUserFinalData, ExampleUserSaveData>
{
    public string? LanguageCode { get; set; }
    public ExampleUserSaveData Save() => new() { LanguageCode = LanguageCode };
    public static ExampleUserFinalData Load(ExampleUserSaveData data) => new() { LanguageCode = data.LanguageCode };
}