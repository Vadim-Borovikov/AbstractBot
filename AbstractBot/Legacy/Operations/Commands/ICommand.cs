using System;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations.Commands;

// ReSharper disable once MemberCanBeInternal
public interface ICommand
{
    public BotCommand BotCommand { get; }
    public bool ShowInMenu { get; }
    public Enum? AccessRequired { get; }
}