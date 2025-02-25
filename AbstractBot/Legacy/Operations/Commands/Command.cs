using System;
using System.Linq;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models.MessageTemplates;
using AbstractBot.Utilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Legacy.Operations.Commands;

[PublicAPI]
public abstract class Command<T> : Operation<T>, ICommand
    where T : class, ICommandData<T>
{
    public BotCommand BotCommand { get; init; }

    [PublicAPI]
    public virtual bool ShowInMenu => true;

    protected Command(MessageTemplateText descriptionFormat, string command, string description)
        : base(descriptionFormat.Format(command, description))
    {
        BotCommand = new BotCommand
        {
            Command = command,
            Description = description
        };
    }

    protected override bool IsInvokingBy(User self, Message message, User sender, out T? data)
    {
        data = null;
        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text))
        {
            return false;
        }

        string[] splitted = message.Text.Split((char[]?) null, StringSplitOptions.RemoveEmptyEntries);
        if (splitted.Length == 0)
        {
            return false;
        }

        string trigger = GetTrigger(self, message.Chat.IsGroup());
        if (!splitted.First().Equals(trigger, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        data = T.From(message, sender, splitted.Skip(1).ToArray());
        return true;
    }

    protected string GetTrigger(User self, bool isGroup)
    {
        return isGroup ? $"/{BotCommand.Command}@{self.Username}" : $"/{BotCommand.Command}";
    }
}