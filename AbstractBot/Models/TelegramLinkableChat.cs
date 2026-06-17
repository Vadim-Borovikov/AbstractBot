using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Models;

[PublicAPI]
public class TelegramLinkableChat
{
    public readonly long Id;
    public readonly string? Username;

    public readonly Chat Chat;
    public readonly Uri? ChatUri;

    public TelegramLinkableChat(long id, string username, ChatType type)
    {
        Id = id;
        Username = username;

        Chat = new Chat
        {
            Id = long.Parse(ChatIdFormat.Format(id)),
            Type = type,
            Username = username
        };

        _messageUriPrefix = ChatUriFormat.Format(username);
        ChatUri = new Uri(_messageUriPrefix);
    }

    public TelegramLinkableChat(long id, Uri? inviteUri, ChatType type)
    {
        Id = id;

        Chat = new Chat
        {
            Id = long.Parse(ChatIdFormat.Format(id)),
            Type = type
        };

        string privateChatUsername = PrivateChatUsernameFormat.Format(id);
        _messageUriPrefix = ChatUriFormat.Format(privateChatUsername);

        ChatUri = inviteUri;
    }

    public Uri GetMesageUri(int messageId) => new(MessageUriFormat.Format(_messageUriPrefix, messageId));

    private const string ChatIdFormat = "-100{0}";
    private const string ChatUriFormat = "https://t.me/{0}";
    private const string PrivateChatUsernameFormat = "c/{0}";
    private const string MessageUriFormat = "{0}/{1}";

    private readonly string _messageUriPrefix;
}