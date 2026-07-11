using System;
using GryphonUtilities.Logging;
using GryphonUtilities.Time;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules;

[PublicAPI]
public sealed class LoggerExtended : Logger
{
    public enum UpdateType
    {
        SendText,
        EditText,
        EditMedia,
        Delete,
        Forward,
        Copy,
        SendPhoto,
        SendSticker,
        Pin,
        Unpin,
        UnpinAll,
        SendInvoice,
        SendFiles,
        ReceiveMessage,
        ReceiveCallback
    }

    public LoggerExtended(Clock clock) : base(clock) { }

    public void LogUpdateRefused(Chat chat, Enum type, string? username = null, int? messageId = null,
        string? data = null)
    {
        string log = GetUpdateLog(chat, type, username, messageId, data);
        Messages.Log($"Refuse to {log}", false);
    }

    public void LogUpdate(Chat chat, Enum type, string? username = null, int? messageId = null, string? data = null)
    {
        string log = GetUpdateLog(chat, type, username, messageId, data);
        Messages.Log($"{log}", false);
    }

    private static string GetUpdateLog(Chat chat, Enum type, string? username = null, int? messageId = null,
        string? data = null)
    {
        string? usernamePart = username is null ? null : $"@{username} ";
        string? messageIdPart = messageId is null ? null : $"message {messageId} ";
        string? dataPart = data is null ? null : $"\"{data.ReplaceLineEndings().Replace(Environment.NewLine, "↵")}\" ";
        return $"{chat.Type} chat {usernamePart}{chat.Id}: {type} {messageIdPart}{dataPart}";
    }
}