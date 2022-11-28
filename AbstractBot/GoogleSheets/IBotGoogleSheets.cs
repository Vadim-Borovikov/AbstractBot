using System;
using JetBrains.Annotations;

namespace AbstractBot.GoogleSheets;

[PublicAPI]
public interface IBotGoogleSheets : IDisposable
{
    public GoogleSheetsComponent GoogleSheetsComponent { get; init; }
}