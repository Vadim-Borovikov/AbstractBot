using System.ComponentModel.DataAnnotations;
using AbstractBot.Models.Config;
using JetBrains.Annotations;

namespace AbstractBot.Example;


internal sealed class ExampleConfig : Config
{
    [Required]
    [UsedImplicitly]
    public int SomeNumber { get; set; }

    [Required]
    [UsedImplicitly]
    public Texts Texts { get; set; } = null!;
}