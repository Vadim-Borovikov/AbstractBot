using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Operations.Infos;
using GryphonUtilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations.Commands;

public sealed class Start<T> : Command<T>
    where T : class, ICommandInfo<T>
{
    private readonly Func<T, Message, User, Task> _onStart;
    protected override byte Order => 0;

    internal Start(BotBasic bot, Func<T, Message, User, Task> onStart)
        : base(bot, "start", bot.Config.Texts.StartCommandDescription)
    {
        _onStart = onStart;
    }

    protected override Task ExecuteAsync(T info, Message message, User sender)
    {
        return _onStart(info, message, sender);
    }

    protected override Task ExecuteAsync(Message message, User sender) => Greet(message.Chat);

    internal async Task Greet(Chat chat)
    {
        await Bot.UpdateCommandsFor(chat.Id);

        string text = $"{Bot.About}";
        if (Bot.Config.Texts.StartPostfixLinesMarkdownV2 is not null)
        {
            text += $"{Environment.NewLine}{Text.JoinLines(Bot.Config.Texts.StartPostfixLinesMarkdownV2)}";
        }
        await Bot.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}