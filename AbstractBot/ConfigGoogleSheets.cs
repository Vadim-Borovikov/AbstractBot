using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public class ConfigGoogleSheets : Config
{
    public readonly string GoogleCredentialJson;

    internal readonly string ApplicationName;
    internal readonly string GoogleSheetId;

    protected ConfigGoogleSheets(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, string googleCredentialJson, string applicationName, string googleSheetId)
        : base(token, systemTimeZoneId, dontUnderstandStickerFileId, forbiddenStickerFileId)
    {
        GoogleCredentialJson = googleCredentialJson;
        ApplicationName = applicationName;
        GoogleSheetId = googleSheetId;
    }
}
