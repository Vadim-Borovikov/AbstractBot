using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace AbstractBot.Models.Config;

[PublicAPI]
public class CommandMenuInfo
{
    [Required]
    [MinLength(1)]
    public string Command { get; init; } = null!;
    [Required]
    [MinLength(1)]
    public string Description { get; init; } = null!;
}