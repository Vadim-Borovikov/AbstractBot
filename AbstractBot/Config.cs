using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public class Config
{
    internal readonly string Token;
    internal readonly string SystemTimeZoneId;
    internal readonly string DontUnderstandStickerFileId;
    internal readonly string ForbiddenStickerFileId;
    internal readonly TimeSpan SendMessagePeriodPrivate;
    internal readonly TimeSpan SendMessagePeriodGroup;
    internal readonly TimeSpan SendMessagePeriodGlobal;

    public string? Host
    {
        init => _host = value;
        get => _host;
    }

    public string? About { get; init; }
    public string? ExtraCommands { get; init; }

    public List<long> AdminIds { protected internal get; init; } = new();

    public long? SuperAdminId { get; init; }

    internal string Url => $"{_host}/{Token}";

    public Config(string token, string systemTimeZoneId, string dontUnderstandStickerFileId,
        string forbiddenStickerFileId, TimeSpan sendMessagePeriodPrivate, TimeSpan sendMessagePeriodGroup,
        TimeSpan sendMessagePeriodGlobal)
    {
        Token = token;
        SystemTimeZoneId = systemTimeZoneId;
        DontUnderstandStickerFileId = dontUnderstandStickerFileId;
        ForbiddenStickerFileId = forbiddenStickerFileId;
        SendMessagePeriodPrivate = sendMessagePeriodPrivate;
        SendMessagePeriodGroup = sendMessagePeriodGroup;
        SendMessagePeriodGlobal = sendMessagePeriodGlobal;
    }

    internal async Task UpdateHostIfNeededAsync(Task<string> task)
    {
        if (string.IsNullOrWhiteSpace(_host))
        {
            _host = await task;
        }
    }

    private string? _host;
}
