using System;
using System.Threading.Tasks;
using System.Threading;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using AbstractBot.Models.MessageTemplates;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models;

[PublicAPI]
public class StatusMessage : IAsyncDisposable
{
    public static Task<StatusMessage> CreateAsync(IUpdateSender bot, Chat chat, MessageTemplateText messageText,
        MessageTemplateText startFormat, MessageTemplateText endFormat, MessageTemplateText postfix)
    {
        return CreateAsync(bot, chat, messageText, startFormat, endFormat, () => postfix);
    }

    public static async Task<StatusMessage> CreateAsync(IUpdateSender bot, Chat chat, MessageTemplateText messageText,
        MessageTemplateText startFormat, MessageTemplateText endFormat,
        Func<MessageTemplateText>? postfixProvider = null)
    {
        MessageTemplateText formatted = startFormat.Format(messageText);
        formatted.KeyboardProvider = KeyboardProvider.Same;
        Message message = await formatted.SendAsync(bot, chat);
        return new StatusMessage(bot, message, formatted, endFormat, postfixProvider, messageText.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        MessageTemplateText? postfix = _postfixProvider?.Invoke();
        MessageTemplateText formatted = _endFormat.Format(_template, postfix);
        formatted.CancellationToken = _cancellationToken;
        await formatted.EditMessageWithSelfAsync(_bot, _message.Chat, _message.MessageId);
    }

    private StatusMessage(IUpdateSender bot, Message message, MessageTemplateText template,
        MessageTemplateText endFormat, Func<MessageTemplateText>? postfixProvider, CancellationToken cancellationToken)
    {
        _bot = bot;
        _message = message;
        _endFormat = endFormat;
        _template = template;
        _postfixProvider = postfixProvider;
        _cancellationToken = cancellationToken;
    }

    private readonly IUpdateSender _bot;
    private readonly MessageTemplateText _endFormat;
    private readonly MessageTemplateText _template;
    private readonly Message _message;
    private readonly Func<MessageTemplateText>? _postfixProvider;
    private readonly CancellationToken _cancellationToken;
}
