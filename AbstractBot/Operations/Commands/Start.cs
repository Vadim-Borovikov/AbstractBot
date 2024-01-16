using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Configs.MessageTemplates;
using AbstractBot.Operations.Data;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public sealed class Start<T> : Command<T>
    where T : class, ICommandData<T>
{
    internal Start(BotBasic bot, Func<T, Message, User, Task> onStart)
        : base(bot, "start", bot.Config.Texts.StartCommandDescription)
    {
        _onStart = onStart;
        _messageTemplate = Bot.Config.Texts.StartFormat;
    }

    public void Format(params object?[] args) => _messageTemplate = Bot.Config.Texts.StartFormat.Format(args);

    protected override Task ExecuteAsync(T data, Message message, User sender)
    {
        return _onStart(data, message, sender);
    }

    protected override Task ExecuteAsync(Message message, User sender) => Greet(message.Chat, sender);

    internal async Task Greet(Chat chat, User sender)
    {
        await Bot.UpdateCommandsFor(sender.Id);

        _messageTemplate.KeyboardProvider = Bot.StartKeyboardProvider;
        await _messageTemplate.SendAsync(Bot, chat);
    }

    private readonly Func<T, Message, User, Task> _onStart;
    private MessageTemplateText _messageTemplate;
}