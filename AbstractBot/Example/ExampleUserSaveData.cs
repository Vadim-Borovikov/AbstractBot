using AbstractBot.Interfaces.Modules.Context;
using JetBrains.Annotations;

namespace AbstractBot.Example;

public sealed class ExampleUserSaveData : ILocalizationUserSaveData
{
    [UsedImplicitly]
    public string? LanguageCode { get; set; }
}