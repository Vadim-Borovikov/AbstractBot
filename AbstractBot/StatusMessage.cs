using System;
using System.Threading.Tasks;
using System.Threading;
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
    public static Task<StatusMessage> CreateAsync(BotBasic bot, Chat chat, MessageTemplate messageText,
        MessageTemplate postfix, int? messageThreadId = null, IEnumerable<MessageEntity>? entities = null,
        bool? disableWebPagePreview = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return CreateAsync(bot, chat, messageText, () => postfix, messageThreadId, entities, disableWebPagePreview,
            protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken);
    }

    public static async Task<StatusMessage> CreateAsync(BotBasic bot, Chat chat, MessageTemplate messageText,
        Func<MessageTemplate>? postfixProvider = null, int? messageThreadId = null,
        IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null, bool? protectContent = null,
        int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default)
    {
        MessageTemplate formatted = bot.Config.Texts.StatusMessageStartFormat.Format(messageText);
        Message message = await formatted.SendAsync(bot, chat, KeyboardProvider.Same, messageThreadId, entities, true,
            protectContent, replyToMessageId, allowSendingWithoutReply, cancellationToken,
            textDisableWebPagePreview: disableWebPagePreview);
        return new StatusMessage(bot, message, formatted, postfixProvider, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        MessageTemplate? postfix = _postfixProvider?.Invoke();
        MessageTemplate formatted = _bot.Config.Texts.StatusMessageEndFormat.Format(_template, postfix);
        string result = formatted.EscapeIfNeeded();
        await _bot.EditMessageTextAsync(_message.Chat, _message.MessageId, result, ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
    }

    private StatusMessage(BotBasic bot, Message message, MessageTemplate template,
        Func<MessageTemplate>? postfixProvider, CancellationToken cancellationToken)
    {
        _bot = bot;
        _template = template;
        _message = message;
        _postfixProvider = postfixProvider;
        _cancellationToken = cancellationToken;
    }

    private readonly BotBasic _bot;
    private readonly MessageTemplate _template;
    private readonly Message _message;
    private readonly Func<MessageTemplate>? _postfixProvider;
    private readonly CancellationToken _cancellationToken;
}
