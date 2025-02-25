using System;
using System.Threading.Tasks;
using System.Threading;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using AbstractBot.Legacy.Bots;
using AbstractBot.Models;
using AbstractBot.Models.MessageTemplates;

namespace AbstractBot.Legacy;

[PublicAPI]
public class StatusMessage : IAsyncDisposable
{
    public static Task<StatusMessage> CreateAsync(BotBasic bot, Chat chat, MessageTemplateText messageText,
        MessageTemplateText postfix)
    {
        return CreateAsync(bot, chat, messageText, () => postfix);
    }

    public static async Task<StatusMessage> CreateAsync(BotBasic bot, Chat chat, MessageTemplateText messageText,
        Func<MessageTemplateText>? postfixProvider = null)
    {
        MessageTemplateText formatted = bot.ConfigBasic.TextsBasic.StatusMessageStartFormat.Format(messageText);
        formatted.KeyboardProvider = KeyboardProvider.Same;
        Message message = await formatted.SendAsync(bot, chat);
        return new StatusMessage(bot, message, formatted, postfixProvider, messageText.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        MessageTemplateText? postfix = _postfixProvider?.Invoke();
        MessageTemplateText formatted = _bot.ConfigBasic.TextsBasic.StatusMessageEndFormat.Format(_template, postfix);
        formatted.CancellationToken = _cancellationToken;
        await formatted.EditMessageWithSelfAsync(_bot, _message.Chat, _message.MessageId);
    }

    private StatusMessage(BotBasic bot, Message message, MessageTemplateText template,
        Func<MessageTemplateText>? postfixProvider, CancellationToken cancellationToken)
    {
        _bot = bot;
        _template = template;
        _message = message;
        _postfixProvider = postfixProvider;
        _cancellationToken = cancellationToken;
    }

    private readonly BotBasic _bot;
    private readonly MessageTemplateText _template;
    private readonly Message _message;
    private readonly Func<MessageTemplateText>? _postfixProvider;
    private readonly CancellationToken _cancellationToken;
}
