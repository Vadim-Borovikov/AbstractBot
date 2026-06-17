using AbstractBot.Interfaces.Modules;
using AbstractBot.Models.MessageTemplates;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot.Models;

[PublicAPI]
public class StatusMessage : IAsyncDisposable
{
    public static Task<StatusMessage> CreateAsync(IUpdateSender updateSender, Chat chat, string messageText,
        string startFormat, string endFormat, string postfix)
    {
        return CreateAsync(updateSender, chat, messageText, startFormat, endFormat, () => postfix);
    }

    public static async Task<StatusMessage> CreateAsync(IUpdateSender updateSender, Chat chat, string messageText,
        string startFormat, string endFormat, Func<string>? postfixProvider = null)
    {
        string formatted = startFormat.Format(messageText);
        MessageTemplateText template = new(formatted)
        {
            KeyboardProvider = KeyboardProvider.Same
        };
        Message message = await template.SendAsync(updateSender, chat);
        return new StatusMessage(updateSender, message, formatted, endFormat, postfixProvider);
    }

    public async ValueTask DisposeAsync()
    {
        string? postfix = _postfixProvider?.Invoke();
        string formatted = _endFormat.Format(_formatted, postfix);
        MessageTemplateText template = new(formatted);
        await template.EditMessageWithSelfAsync(_bot, _message.Chat, _message.MessageId);
    }

    private StatusMessage(IUpdateSender bot, Message message, string formatted, string endFormat,
        Func<string>? postfixProvider)
    {
        _bot = bot;
        _message = message;
        _endFormat = endFormat;
        _formatted = formatted;
        _postfixProvider = postfixProvider;
    }

    private readonly IUpdateSender _bot;
    private readonly string _endFormat;
    private readonly string _formatted;
    private readonly Message _message;
    private readonly Func<string>? _postfixProvider;
}
