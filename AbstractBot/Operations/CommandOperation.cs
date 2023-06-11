using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations;

public abstract class CommandOperation : Operation
{
    [PublicAPI]
    public readonly BotCommand Command;

    protected CommandOperation(Bot bot, string command, string description) : base(bot)
    {
        Command = new BotCommand
        {
            Command = command,
            Description = description
        };
        MenuDescription = $"/{Bot.EscapeCharacters(Command.Command)} – {Bot.EscapeCharacters(Command.Description)}";
    }

    protected internal override async Task<ExecutionResult> TryExecuteAsync(Message message, long senderId)
    {
        if (!IsInvokingBy(message, out string? payload))
        {
            return ExecutionResult.UnsuitableOperation;
        }

        if (!IsAccessSuffice(senderId))
        {
            return ExecutionResult.InsufficentAccess;
        }

        await ExecuteAsync(message, senderId, payload);
        return ExecutionResult.Success;
    }

    protected abstract Task ExecuteAsync(Message message, long senderId, string? payload);

    [PublicAPI]
    protected virtual bool IsInvokingBy(Message message, out string? payload)
    {
        payload = null;
        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text))
        {
            return false;
        }

        string mainPart =
            message.Chat.IsGroup() ? $"/{Command.Command}@{Bot.User?.Username}" : $"/{Command.Command}";

        if (message.Text == mainPart)
        {
            return true;
        }

        if (!message.Text.StartsWith(mainPart, StringComparison.Ordinal))
        {
            return false;
        }

        string? postfix = message.Text[mainPart.Length..];
        if (postfix is null)
        {
            return false;
        }

        payload = postfix.Trim();
        return postfix[0] != payload[0];
    }
}