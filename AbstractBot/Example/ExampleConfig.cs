using System.ComponentModel.DataAnnotations;
using AbstractBot.Models.Config;
using JetBrains.Annotations;

namespace AbstractBot.Example;

[PublicAPI]
internal sealed class ExampleConfig : Config
{
    [Required]
    [MinLength(1)]
    public string SavePath { get; set; } = null!;

    [Required]
    [UsedImplicitly]
    public Texts Texts { get; set; } = null!;

    [Required]
    [UsedImplicitly]
    public int SomeNumber { get; set; }
}