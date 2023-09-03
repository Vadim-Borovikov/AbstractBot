using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace AbstractBot.Configs;

[PublicAPI]
public class Texts
{
    public List<string?> AboutLines { get; init; } = null!;
    public List<string?>? StartPostfixLines { get; init; }
    public List<string?>? HelpPrefixLines { get; init; }
}