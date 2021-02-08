using System;
using System.Diagnostics.CodeAnalysis;
using GoogleSheetsManager;
using Newtonsoft.Json;
namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class BotBaseGoogleSheets<TConfig> : BotBase<TConfig>, IDisposable
        where TConfig: ConfigGoogleSheets
    {
        protected BotBaseGoogleSheets(TConfig config) : base(config)
        {
            string googleCredentialJson = JsonConvert.SerializeObject(Config.GoogleCredential);
            GoogleSheetsProvider = new Provider(googleCredentialJson, Config.ApplicationName, Config.GoogleSheetId);
        }

        public virtual void Dispose() => GoogleSheetsProvider?.Dispose();

        public readonly Provider GoogleSheetsProvider;
    }
}