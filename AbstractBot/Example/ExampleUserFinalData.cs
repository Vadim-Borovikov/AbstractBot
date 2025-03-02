using AbstractBot.Interfaces.Modules.Context;

namespace AbstractBot.Example;

public sealed class ExampleUserFinalData : IUserFinalData<ExampleUserFinalData, ExampleUserSaveData>
{
    public string? LanguageCode;
    public ExampleUserSaveData Save() => new() { LanguageCode = LanguageCode };
    public static ExampleUserFinalData Load(ExampleUserSaveData data) => new() { LanguageCode = data.LanguageCode };
}