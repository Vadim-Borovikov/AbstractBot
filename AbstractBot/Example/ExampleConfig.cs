using AbstractBot.Models.Config;
using JetBrains.Annotations;

namespace AbstractBot.Example;


internal sealed class ExampleConfig : Config
{
    [UsedImplicitly]
    public int SomeNumber { get; set; }
}