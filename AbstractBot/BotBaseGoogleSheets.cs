using System;
using GoogleSheetsManager.Providers;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot;

[PublicAPI]
public abstract class BotBaseGoogleSheets<TBot, TConfig> : BotBaseCustom<TConfig>, IDisposable
    where TBot: BotBaseGoogleSheets<TBot, TConfig>
    where TConfig : ConfigGoogleSheets
{
    public readonly SheetsProvider GoogleSheetsProvider;

    protected BotBaseGoogleSheets(TConfig config) : base(config)
    {
        string json = string.IsNullOrWhiteSpace(Config.GoogleCredentialJson)
            ? JsonConvert.SerializeObject(Config.GoogleCredential)
            : Config.GoogleCredentialJson;
        GoogleSheetsProvider = new SheetsProvider(json, Config.ApplicationName, Config.GoogleSheetId);
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}