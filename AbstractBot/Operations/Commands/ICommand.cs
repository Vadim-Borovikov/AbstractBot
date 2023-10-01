﻿using Telegram.Bot.Types;

namespace AbstractBot.Operations.Commands;

internal interface ICommand
{
    public BotCommand BotCommand { get; }
    public bool ShowInMenu { get; }
    public int AccessRequired { get; }
}