using System;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot;

[PublicAPI]
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
        Unpin
    }

    public static void LogRefused(Chat chat, Type type, int? messageId = null, string? data = null)
    {
        string log = GetLog(chat, type, messageId, data);
        Utils.LogManager.LogTimedMessage($"Refuse to {log}");
    }

    internal static void Log(Chat chat, Type type, int? messageId = null, string? data = null)
    {
        string log = GetLog(chat, type, messageId, data);
        Utils.LogManager.LogTimedMessage(log);
    }

    private static string GetLog(Chat chat, Type type, int? messageId = null, string? data = null)
    {
        string? messageIdPart = messageId is null ? null : $"message {messageId} ";
        string? dataPart = data is null ? null : $"\"{data.ReplaceLineEndings().Replace(Environment.NewLine, "↵")}\" ";
        return $"{type} {messageIdPart}{dataPart}in {chat.Type} chat {chat.Id}";
    }
}
