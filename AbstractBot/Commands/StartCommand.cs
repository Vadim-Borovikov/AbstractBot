﻿using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

public sealed class StartCommand : CommandBase
{
    public StartCommand(BotBase bot) : base(bot, "start", "приветствие") { }

    public override async Task ExecuteAsync(Message message, Chat chat, string? payload)
    {
        string text = $"{BotBase.About}";
        if (!string.IsNullOrWhiteSpace(BotBase.StartPostfix))
        {
            text += $"{Environment.NewLine}{BotBase.StartPostfix}";
        }
        await BotBase.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}