﻿using System;
using System.Linq;
using AbstractBot.Bots;
using AbstractBot.Extensions;
using AbstractBot.Operations.Infos;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class Command<T> : Operation<T>, ICommand
    where T : class, ICommandInfo<T>
{
    public BotCommand BotCommand { get; init; }


    [PublicAPI]
    public virtual bool HideFromMenu => false;

    protected Command(BotBasic bot, string command, string description) : base(bot)
    {
        BotCommand = new BotCommand
        {
            Command = command,
            Description = description
        };
        MenuDescription = $"/{BotCommand.Command} – {BotCommand.Description.Escape()}";
    }

    protected override bool IsInvokingBy(Message message, User sender, out T? info)
    {
        info = null;
        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text))
        {
            return false;
        }

        string[] splitted = message.Text.Split((char[]?) null, StringSplitOptions.RemoveEmptyEntries);
        if (splitted.Length == 0)
        {
            return false;
        }

        string trigger = GetTrigger(message.Chat.IsGroup());
        if (!splitted.First().Equals(trigger, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        if (splitted.Length == 1)
        {
            return true;
        }

        info = T.From(splitted.Skip(1).ToArray());
        return info is not null;
    }

    protected string GetTrigger(bool isGroup)
    {
        return isGroup ? $"/{BotCommand.Command}@{Bot.User?.Username}" : $"/{BotCommand.Command}";
    }
}