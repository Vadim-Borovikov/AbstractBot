using System;
using System.Collections.Generic;
using System.Linq;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Models.MessageTemplates;
using AbstractBot.Utilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public sealed class BotCommandExtended : BotCommand
{
    public bool ShowInMenu { get; }

    internal BotCommandExtended(string start, string description, string selfUsername,
        ITextsProvider<ITexts> textsProvider, bool showInMenu = true)
        : base(start, description)
    {
        _selfUsername = selfUsername;
        _textsProvider = textsProvider;
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

    public MessageTemplateText? GetHelpDescriptionFor(long userId)
    {
        ITexts texts = _textsProvider.GetTextsFor(userId);
        string? description = texts.TryGetMenuDescription(Command);
        return description is null ? null : texts.CommandDescriptionFormat.Format(Command, description);
    }

    private readonly string _selfUsername;
    private readonly ITextsProvider<ITexts> _textsProvider;
}