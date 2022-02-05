using System;
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
        GoogleSheetsProvider = new SheetsProvider(googleCredentialJson,
            Config.ApplicationName ?? throw new NullReferenceException(nameof(Config.ApplicationName)),
            Config.GoogleSheetId ?? throw new NullReferenceException(nameof(Config.GoogleSheetId)));
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    public readonly SheetsProvider GoogleSheetsProvider;
}