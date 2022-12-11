using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations;

public abstract class CommandOperation : Operation
{
    protected internal readonly BotCommand Command;

    protected CommandOperation(BotBase bot, string command, string description) : base(bot)
    {
        Command = new BotCommand
        {
            Command = command,
            Description = description
        };
        MenuDescription =
            $"/{Utils.EscapeCharacters(Command.Command)} – {Utils.EscapeCharacters(Command.Description)}";
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

    private bool IsInvokingBy(Message message, out string? payload)
    {
        payload = null;
        if ((message.Type != MessageType.Text) || string.IsNullOrWhiteSpace(message.Text))
        {
            return false;
        }

        string mainPart =
            Utils.IsGroup(message.Chat) ? $"/{Command.Command}@{BotBase.User?.Username}" : $"/{Command.Command}";

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