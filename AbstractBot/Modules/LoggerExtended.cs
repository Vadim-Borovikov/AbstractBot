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

    public void LogUpdateRefused(Chat chat, Enum type, int? messageId = null,
        string? data = null)
    {
        string log = GetUpdateLog(chat, type, messageId, data);
        Messages.Log($"Refuse to {log}", false);
    }

    public void LogUpdate(Chat chat, Enum type, int? messageId = null, string? data = null)
    {
        string log = GetUpdateLog(chat, type, messageId, data);
        Messages.Log($"Refuse to {log}", false);
    }

    private static string GetUpdateLog(Chat chat, Enum type, int? messageId = null, string? data = null)
    {
        string? messageIdPart = messageId is null ? null : $"message {messageId} ";
        string? dataPart = data is null ? null : $"\"{data.ReplaceLineEndings().Replace(Environment.NewLine, "↵")}\" ";
        return $"{chat.Type} chat {chat.Id}: {type} {messageIdPart}{dataPart}";
    }
}