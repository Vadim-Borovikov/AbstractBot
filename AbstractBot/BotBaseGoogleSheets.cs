using System;
using GoogleSheetsManager.Providers;
using JetBrains.Annotations;
namespace AbstractBot;

[PublicAPI]
public abstract class BotBaseGoogleSheets<TBot, TConfig> : BotBase<TBot, TConfig>, IDisposable
    where TBot: BotBaseGoogleSheets<TBot, TConfig>
    where TConfig : ConfigGoogleSheets
{
    protected BotBaseGoogleSheets(TConfig config) : base(config)
    {
        GoogleSheetsProvider =
            new SheetsProvider(Config.GoogleCredentialJson, Config.ApplicationName, Config.GoogleSheetId);
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    public readonly SheetsProvider GoogleSheetsProvider;
}
