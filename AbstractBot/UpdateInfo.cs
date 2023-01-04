using System;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot;

public static class UpdateInfo
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
        SendInvoice,
        SendFiles
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
        return $"{type} {messageIdPart}{dataPart}in {chat.Type} chat {chat.Id}";
    }
}
