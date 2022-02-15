using System;
using GoogleSheetsManager;
using GoogleSheetsManager.Providers;
using JetBrains.Annotations;
using Newtonsoft.Json;
namespace AbstractBot;

[PublicAPI]
public abstract class BotBaseGoogleSheets<TBot, TConfig> : BotBase<TBot, TConfig>, IDisposable
    where TBot: BotBaseGoogleSheets<TBot, TConfig>
    where TConfig : ConfigGoogleSheets
{
    protected BotBaseGoogleSheets(TConfig config) : base(config)
    {
        string googleCredentialJson = JsonConvert.SerializeObject(Config.GoogleCredential);
        string applicationName = Config.ApplicationName.GetValue(nameof(Config.ApplicationName));
        string sheetId = Config.ApplicationName.GetValue(nameof(Config.GoogleSheetId));
        GoogleSheetsProvider = new SheetsProvider(googleCredentialJson, applicationName, sheetId);
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    public readonly SheetsProvider GoogleSheetsProvider;
}
