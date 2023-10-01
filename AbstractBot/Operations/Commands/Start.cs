using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Configs;
using AbstractBot.Operations.Infos;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public sealed class Start<T> : Command<T>
    where T : class, ICommandInfo<T>
{
    private readonly Func<T, Message, User, Task> _onStart;

    internal Start(BotBasic bot, Func<T, Message, User, Task> onStart)
        : base(bot, "start", bot.Config.Texts.StartCommandDescription)
    {
        _onStart = onStart;
        _messageTemplate = Bot.Config.Texts.StartFormat;
    }

    public void Format(params object?[] args) => _messageTemplate = Bot.Config.Texts.StartFormat.Format(args);

    protected override Task ExecuteAsync(T info, Message message, User sender)
    {
        return _onStart(info, message, sender);
    }

    protected override Task ExecuteAsync(Message message, User sender) => Greet(message.Chat, sender);

    internal async Task Greet(Chat chat, User sender)
    {
        await Bot.UpdateCommandsFor(sender.Id);

        await _messageTemplate.SendAsync(Bot, chat);
    }

    private MessageTemplate _messageTemplate;
}