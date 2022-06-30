using System;
using Telegram.Bot.Types;

namespace AbstractBot;
internal sealed class UpdateInfo
{
    public enum Type
    {
        SendText,
        EditText,
        Delete,
        SendPhoto,
        SendSticker
    }

    public readonly Chat Chat;
    public readonly DateTime Sent;

    public UpdateInfo(Chat chat, DateTime sent, Type type, int? messageId = null, string? data = null)
    {
        Chat = chat;
        Sent = sent;

        _type = type;
        _messageId = messageId;
        _data = data;
    }

    public override string ToString()
    {
        string? messageId = _messageId is null ? null : $"'Message {_messageId} ";
        string? data = _data is null ? null : $"\"{_data}\" ";
        return $"{messageId}{_type} {data}sent to {Chat.Type} {Chat.Id} at {Sent:T}";
    }

    private readonly Type _type;
    private readonly int? _messageId;
    private readonly string? _data;
}
