﻿using System;
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

    protected override Task ExecuteAsync(Message message, User sender) => Greet(message.Chat, sender);

    internal async Task Greet(Chat chat, User sender)
    {
        await Bot.UpdateCommandsFor(sender.Id);

        string text = Bot.Config.Texts.StartLinesFormatMarkdownV2 is null
            ? Bot.About
            : Text.FormatLines(Bot.Config.Texts.StartLinesFormatMarkdownV2, Bot.About);

        await Bot.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}