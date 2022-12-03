using System;
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
    public static async Task<StatusMessage> CreateAsync(BotBase bot, Chat chat, string text,
        bool? disableWebPagePreview = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, string? postfix = null, CancellationToken cancellationToken = default)
    {
        text = $"_{Utils.EscapeCharacters(text)}…_";
        Message message = await bot.SendTextMessageAsync(chat, text, null, ParseMode.MarkdownV2,
            disableWebPagePreview: disableWebPagePreview, disableNotification: true, protectContent: protectContent,
            replyToMessageId: replyToMessageId, allowSendingWithoutReply: allowSendingWithoutReply,
            cancellationToken: cancellationToken);
        return new StatusMessage(bot, message, postfix, cancellationToken);
    }

    private StatusMessage(BotBase bot, Message message, string? postfix, CancellationToken cancellationToken)
    {
        _bot = bot;
        _message = message;
        _postfix = postfix;
        _cancellationToken = cancellationToken;
    }

    public ValueTask DisposeAsync()
    {
        string text = _message.Text.GetValue(nameof(_message.Text));
        text = $"_{Utils.EscapeCharacters(text)}_ Готово\\.{_postfix}";
        Task<Message> task = _bot.EditMessageTextAsync(_message.Chat, _message.MessageId, text, ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken);
        return new ValueTask(task);
    }

    private readonly BotBase _bot;
    private readonly Message _message;
    private readonly string? _postfix;
    private readonly CancellationToken _cancellationToken;
}
