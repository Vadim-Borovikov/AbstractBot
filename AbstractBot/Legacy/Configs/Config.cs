using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AbstractBot.Legacy.Configs;

[PublicAPI]
public class Config<T> : ConfigBasic
    where T : TextsBasic
{
    [Required]
    public T Texts { get; init; } = null!;

    public override TextsBasic TextsBasic => Texts;
}