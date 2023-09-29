using System;
using System.Threading.Tasks;
using System.Threading;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using AbstractBot.Bots;
using System.Collections.Generic;
using AbstractBot.Configs;

namespace AbstractBot;

[PublicAPI]
public class StatusMessage : IAsyncDisposable
{
    public static Task<StatusMessage> CreateAsync(BotBasic bot, Chat chat, MessageText messageText,
        MessageText postfix, int? messageThreadId = null, IEnumerable<MessageEntity>? entities = null,
        bool? disableWebPagePreview = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(bot, chat, messageText, () => postfix, messageThreadId, entities, disableWebPagePreview,
            protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public static async Task<StatusMessage> CreateAsync(BotBasic bot, Chat chat, MessageText messageText,
        Func<MessageText>? postfixProvider = null, int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        MessageText formatted = bot.Config.Texts.StatusMessageStartFormat.Format(messageText);
        Message message = await formatted.SendAsync(bot, chat, KeyboardProvider.Same, messageThreadId, entities,
            disableWebPagePreview, true, protectContent, replyToMessageId, allowSendingWithoutReply,
            cancellationToken);
        return new StatusMessage(bot, message, postfixProvider, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        string text = _message.Text.Denull(nameof(_message.Text));
        MessageText? postfix = _postfixProvider?.Invoke();
        MessageText formatted = _bot.Config.Texts.StatusMessageEndFormat.Format(text, postfix);
        string result = formatted.EscapeIfNeeded();
        await _bot.EditMessageTextAsync(_message.Chat, _message.MessageId, result, ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    private StatusMessage(BotBasic bot, Message message, Func<MessageText>? postfixProvider,
        CancellationToken cancellationToken)
    {
        _bot = bot;
        _message = message;
        _postfixProvider = postfixProvider;
        _cancellationToken = cancellationToken;
    }

    private readonly BotBasic _bot;
    private readonly Message _message;
    private readonly Func<MessageText>? _postfixProvider;
    private readonly CancellationToken _cancellationToken;
}
