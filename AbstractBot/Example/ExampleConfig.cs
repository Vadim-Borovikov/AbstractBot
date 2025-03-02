using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Models.Config;
using JetBrains.Annotations;

namespace AbstractBot.Example;

[PublicAPI]
internal sealed class ExampleConfig : Config, ILocalizationConfig<Texts>
{
    [Required]
    [MinLength(1)]
    public string SavePath { get; set; } = null!;

    [Required]
    [UsedImplicitly]
    public int SomeNumber { get; set; }

    [Required]
    public Dictionary<string, Texts> AllTexts { get; set; } = null!;

    [Required]
    public string DefaultLanguageCode { get; set; } = null!;
}