using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AbstractBot.Configs;

[PublicAPI]
public class Config<T> : ConfigBasic
    where T : Texts
{
    [Required]
    public new T Texts { get; init; } = null!;
}