﻿using System;
using System.Threading.Tasks;
using System.Threading;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot;

[PublicAPI]
public class StatusMessage : IAsyncDisposable
{
    public static Task<StatusMessage> CreateAsync(BotBase bot, Chat chat, string text,
        string postfix, bool? disableWebPagePreview = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(bot, chat, text, () => postfix, disableWebPagePreview, protectContent, replyToMessageId,
            allowSendingWithoutReply, cancellationToken);
    }

    public static async Task<StatusMessage> CreateAsync(BotBase bot, Chat chat, string text,
        Func<string>? postfixProvider = null, bool? disableWebPagePreview = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        text = $"_{Utils.EscapeCharacters(text)}…_";
        Message message = await bot.SendTextMessageAsync(chat, text, null, ParseMode.MarkdownV2,
            disableWebPagePreview: disableWebPagePreview, disableNotification: true, protectContent: protectContent,
            replyToMessageId: replyToMessageId, allowSendingWithoutReply: allowSendingWithoutReply,
            cancellationToken: cancellationToken);
        return new StatusMessage(bot, message, postfixProvider, cancellationToken);
    }

    private StatusMessage(BotBase bot, Message message, Func<string>? postfixProvider,
        CancellationToken cancellationToken)
    {
        _bot = bot;
        _message = message;
        _postfixProvider = postfixProvider;
        _cancellationToken = cancellationToken;
    }

    public async ValueTask DisposeAsync()
    {
        string text = _message.Text.GetValue(nameof(_message.Text));
        string? postfix = _postfixProvider?.Invoke();
        text = $"_{Utils.EscapeCharacters(text)}_ Готово\\.{postfix}";
        await _bot.EditMessageTextAsync(_message.Chat, _message.MessageId, text, ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    private readonly BotBase _bot;
    private readonly Message _message;
    private readonly Func<string>? _postfixProvider;
    private readonly CancellationToken _cancellationToken;
}