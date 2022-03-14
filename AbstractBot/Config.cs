using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public class Config
{
    internal readonly string Token;
    internal readonly string SystemTimeZoneId;
    internal readonly string DontUnderstandStickerFileId;
    internal readonly string ForbiddenStickerFileId;
    internal readonly TimeSpan SendMessageDelay;

    public string? Host { get; set; }
    public string? About { get; init; }
    public string? ExtraCommands { get; init; }
    public List<long>? AdminIds { get; init; }
    public long? SuperAdminId { get; init; }

    internal string Url => $"{Host}/{Token}";

    internal Config(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, TimeSpan sendMessageDelay)
    {
        Token = token;
        SystemTimeZoneId = systemTimeZoneId;
        DontUnderstandStickerFileId = dontUnderstandStickerFileId;
        ForbiddenStickerFileId = forbiddenStickerFileId;
        SendMessageDelay = sendMessageDelay;
    }
}
