using System;
using System.Threading.Tasks;
using AbstractBot.Legacy.Bots;
using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Configs.MessageTemplates;
using AbstractBot.Legacy.Operations.Data;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations.Commands;

[PublicAPI]
public sealed class Start<T> : Command<T>
    where T : class, ICommandData<T>
{
    internal Start(TextsBasic texts, Func<T, Message, User, Task> onStart)
        : base(texts.CommandDescriptionFormat, "start", texts.StartCommandDescription)
    {
        _onStart = onStart;
        _messageTemplateFormat = texts.StartFormat;
        _messageTemplate = new MessageTemplateText(_messageTemplateFormat);
    }

    public void Format(params object?[] args) => _messageTemplate = _messageTemplateFormat.Format(args);

    protected override Task ExecuteAsync(BotBasic bot, T data, Message message, User sender)
    {
        return _onStart(data, message, sender);
    }

    protected override Task ExecuteAsync(BotBasic bot, Message message, User sender)
    {
        return Greet(bot, message.Chat, sender);
    }

    internal async Task Greet(BotBasic bot, Chat chat, User sender)
    {
        await bot.UpdateCommandsFor(sender.Id);

        _messageTemplate.KeyboardProvider = bot.StartKeyboardProvider;
        await _messageTemplate.SendAsync(bot, chat);
    }

    private readonly Func<T, Message, User, Task> _onStart;
    private MessageTemplateText _messageTemplate;
    private MessageTemplateText _messageTemplateFormat;
}