using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace AbstractBot.Configs;

[PublicAPI]
public class Config<T> : ConfigBasic
    where T : Texts
{
    [Required]
    public new T Texts { get; init; } = null!;
}