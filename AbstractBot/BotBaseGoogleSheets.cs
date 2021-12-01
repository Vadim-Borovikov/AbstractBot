using System;
using System.Diagnostics.CodeAnalysis;
using GoogleSheetsManager.Providers;
using Newtonsoft.Json;
namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class BotBaseGoogleSheets<TBot, TConfig> : BotBase<TBot, TConfig>, IDisposable
        where TBot: BotBaseGoogleSheets<TBot, TConfig>
        where TConfig : ConfigGoogleSheets
    {
        protected BotBaseGoogleSheets(TConfig config) : base(config)
        {
            string googleCredentialJson = JsonConvert.SerializeObject(Config.GoogleCredential);
            GoogleSheetsProvider = new SheetsProvider(googleCredentialJson, Config.ApplicationName, Config.GoogleSheetId);
        }

        public virtual void Dispose() => GoogleSheetsProvider?.Dispose();

        public readonly SheetsProvider GoogleSheetsProvider;
    }
}