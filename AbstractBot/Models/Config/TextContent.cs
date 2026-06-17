using AbstractBot.Utilities.Extensions;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace AbstractBot.Models.Config;

[PublicAPI]
public sealed class TextContent
{
    public required IEnumerable<string> Text { get; init; }
    public bool Escaped { get; init; }

    public override string ToString()
    {
        if (_escapedText is null)
        {
            string joined = Text.JoinLines();
            _escapedText = Escaped ? joined : joined.Escape();
        }
        return _escapedText;
    }

    public string Format(params object?[] args) => ToString().Format(args);

#pragma warning disable IDE0032
    private string? _escapedText;
#pragma warning restore IDE0032
}