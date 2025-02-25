using System;
using System.Collections.Generic;
using System.Linq;
using AbstractBot.Utilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public sealed class BotCommandExtended : BotCommand
{
    public bool ShowInMenu { get; }

    internal BotCommandExtended(string start, string description, string selfUsername, bool showInMenu = true)
        : base(start, description)
    {
        _selfUsername = selfUsername;
        ShowInMenu = showInMenu;
    }

    public IEnumerable<string>? TryGetParameters(Message message)
    {
        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text))
        {
            return null;
        }

        string[] splitted = message.Text.Split((char[]?) null, StringSplitOptions.RemoveEmptyEntries);
        if (splitted.Length == 0)
        {
            return null;
        }

        string trigger = message.Chat.IsGroup() ? $"/{Command}@{_selfUsername}" : $"/{Command}";
        return splitted.First().Equals(trigger, StringComparison.InvariantCultureIgnoreCase) ? splitted.Skip(1) : null;
    }

    private readonly string _selfUsername;
}