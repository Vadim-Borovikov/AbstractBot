using System;
using AbstractBot.Bots;
using AbstractBot.Extensions;
using AbstractBot.Operations.Infos;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class Command<T> : Operation<T>, ICommand
    where T : InfoCommand
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

        string trigger =
            message.Chat.IsGroup() ? $"/{BotCommand.Command}@{Bot.User?.Username}" : $"/{BotCommand.Command}";
        if (!message.Text.StartsWith(trigger, StringComparison.Ordinal))
        {
            return false;
        }

        string payload = message.Text[trigger.Length..];
        return IsInvokingByPayload(message, sender, payload, out info);
    }

    protected abstract bool IsInvokingByPayload(Message message, User sender, string payload, out T info);}