using System;
using System.Collections.Generic;
using System.Text.Json;
using GoogleSheetsManager.Providers;
using GryphonUtilities;
using JetBrains.Annotations;

namespace AbstractBot.GoogleSheets;

[PublicAPI]
public class GoogleSheetsComponent : IDisposable
{
    public readonly SheetsProvider GoogleSheetsProvider;
    public readonly Dictionary<Type, Func<object?, object?>> AdditionalConverters;

    public GoogleSheetsComponent(IConfigGoogleSheets config, JsonSerializerOptions options, TimeManager timeManager)
    {
        string json = string.IsNullOrWhiteSpace(config.GoogleCredentialJson)
            ? JsonSerializer.Serialize(config.GoogleCredential, options)
            : config.GoogleCredentialJson;
        GoogleSheetsProvider = new SheetsProvider(json, config.ApplicationName, config.GoogleSheetId);
        AdditionalConverters = new Dictionary<Type, Func<object?, object?>>
        {
            { typeof(DateTimeFull), o => GetDateTimeFull(o) },
            { typeof(DateTimeFull?), o => GetDateTimeFull(o) }
        };
        _timeManager = timeManager;
    }

    public DateTimeFull? GetDateTimeFull(object? o)
    {
        switch (o)
        {
            case DateTimeFull dtf: return dtf;
            case DateTimeOffset dto: return _timeManager.GetDateTimeFull(dto);
            default:
            {
                DateTime? dt = GoogleSheetsManager.Utils.GetDateTime(o);
                return dt is null ? null : _timeManager.GetDateTimeFull(dt.Value);
            }
        }
    }

    public virtual void Dispose()
    {
        GoogleSheetsProvider.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly TimeManager _timeManager;
}