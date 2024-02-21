using System;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Logging;

public static class Update
{
    public enum Type
    {
        SendText,
        EditText,
        Delete,
        Forward,
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

    [PublicAPI]
    public static void LogRefused(Chat chat, Type type, Logger logger, int? messageId = null, string? data = null)
    {
        string log = GetLog(chat, type, messageId, data);
        logger.LogTimedMessage($"Refuse to {log}");
    }

    internal static void Log(Chat chat, Type type, Logger logger, int? messageId = null, string? data = null)
    {
        string log = GetLog(chat, type, messageId, data);
        logger.LogTimedMessage(log);
    }

    private static string GetLog(Chat chat, Type type, int? messageId = null, string? data = null)
    {
        string? messageIdPart = messageId is null ? null : $"message {messageId} ";
        string? dataPart = data is null ? null : $"\"{data.ReplaceLineEndings().Replace(Environment.NewLine, "↵")}\" ";
        return $"{chat.Type} chat {chat.Id}: {type} {messageIdPart}{dataPart}";
    }
}
