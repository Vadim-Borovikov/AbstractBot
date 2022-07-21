using System;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public class ConfigGoogleSheets : Config
{
    public readonly string GoogleCredentialJson;
    public readonly string ApplicationName;

    internal readonly string GoogleSheetId;

    protected ConfigGoogleSheets(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, TimeSpan sendMessagePeriodPrivate, TimeSpan sendMessagePeriodGroup,
        TimeSpan sendMessagePeriodGlobal, string googleCredentialJson, string applicationName, string googleSheetId)
        : base(token, systemTimeZoneId, dontUnderstandStickerFileId, forbiddenStickerFileId, sendMessagePeriodPrivate,
            sendMessagePeriodGroup, sendMessagePeriodGlobal)
    {
        GoogleCredentialJson = googleCredentialJson;
        ApplicationName = applicationName;
        GoogleSheetId = googleSheetId;
    }
}
