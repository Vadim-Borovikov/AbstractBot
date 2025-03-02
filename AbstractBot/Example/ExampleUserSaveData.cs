using AbstractBot.Interfaces.Modules.Context;
using JetBrains.Annotations;

namespace AbstractBot.Example;

public sealed class ExampleUserSaveData : IUserSaveData
{
    [UsedImplicitly]
    public string? LanguageCode { get; set; }
}