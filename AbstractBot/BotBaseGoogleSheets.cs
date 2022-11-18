using System;
using System.Collections.Generic;
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
            { typeof(DateTimeFull), o => GetDateTimeFull(o) },
            { typeof(DateTimeFull?), o => GetDateTimeFull(o) }
        };
    }

    public DateTimeFull? GetDateTimeFull(object? o)
    {
        switch (o)
        {
            case DateTimeFull dtf: return dtf;
            case DateTimeOffset dto: return TimeManager.GetDateTimeFull(dto);
            default:
            {
                DateTime? dt = GoogleSheetsManager.Utils.GetDateTime(o);
                return dt is null ? null : TimeManager.GetDateTimeFull(dt.Value);
            }
        }
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }
}