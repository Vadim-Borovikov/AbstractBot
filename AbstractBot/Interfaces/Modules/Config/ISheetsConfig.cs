using System.Collections.Generic;
using GoogleSheetsManager;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Config;

[PublicAPI]
public interface ISheetsConfig : IConfig, IConfigGoogleSheets
{
    Dictionary<string, string> GoogleCredential { get; }

    Dictionary<string, string> IConfigGoogleSheets.Credential => GoogleCredential;
    string IConfigGoogleSheets.TimeZoneId => SystemTimeZoneId;
}