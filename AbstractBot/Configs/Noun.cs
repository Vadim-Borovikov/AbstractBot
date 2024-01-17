using System.ComponentModel.DataAnnotations;
using GryphonUtilities.Helpers;
using JetBrains.Annotations;

namespace AbstractBot.Configs;

[PublicAPI]
public class Noun
{
    [Required]
    [MinLength(1)]
    public string Form1 { get; init; } = null!;
    [Required]
    [MinLength(1)]
    public string Form24 { get; init; } = null!;
    [Required]
    [MinLength(1)]
    public string FormAlot { get; init; } = null!;

    public string FormatWithNumeric(string format, uint number)
    {
        return Text.FormatNumericWithNoun(format, number, Form1, Form24, FormAlot);
    }
}