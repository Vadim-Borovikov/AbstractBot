using System;
using System.Collections.Generic;
using GoogleSheetsManager;
using GoogleSheetsManager.Providers;
using GryphonUtilities;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot;

[PublicAPI]
public abstract class BotBaseGoogleSheets<TBot, TConfig> : BotBaseCustom<TConfig>, IDisposable
    where TBot: BotBaseGoogleSheets<TBot, TConfig>
    where TConfig : ConfigGoogleSheets
{
    public readonly SheetsProvider GoogleSheetsProvider;
    public readonly Dictionary<Type, Func<object?, object?>> AdditionalConverters;

    protected BotBaseGoogleSheets(TConfig config) : base(config)
    {
        string json = string.IsNullOrWhiteSpace(Config.GoogleCredentialJson)
            ? JsonConvert.SerializeObject(Config.GoogleCredential)
            : Config.GoogleCredentialJson;
        GoogleSheetsProvider = new SheetsProvider(json, Config.ApplicationName, Config.GoogleSheetId);
        AdditionalConverters = new Dictionary<Type, Func<object?, object?>>
        {
            { typeof(DateTimeFull), o => o.ToDateTimeFull(TimeManager.TimeZoneInfo) },
            { typeof(DateTimeFull?), o => o.ToDateTimeFull(TimeManager.TimeZoneInfo) }
        };
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}