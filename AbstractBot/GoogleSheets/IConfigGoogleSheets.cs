using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.GoogleSheets;

[PublicAPI]
public interface IConfigGoogleSheets
{
    public Dictionary<string, string>? GoogleCredential { get; init; }

    public string? GoogleCredentialJson { get; init; }

    public string ApplicationName { get; init; }

    public string GoogleSheetId { get; init; }
}