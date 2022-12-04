using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Commands;

[PublicAPI]
public abstract class CommandBase : BotCommand
{
    public virtual BotBase.AccessType Access => BotBase.AccessType.Users;

    protected CommandBase(BotBase bot, string command, string description)
    {
        BotBase = bot;
        Command = command;
        Description = description;
    }

    public virtual bool IsInvokingBy(string? text, bool fromGroup, string? botName, out string? payload)
    {
        if (!fromGroup)
        {
            payload = text is null ? null : Utils.GetPostfix(text, $"/{Command} ");
            if (!string.IsNullOrWhiteSpace(payload))
            {
                return true;
            }
        }

        payload = null;

        return text == (fromGroup ? $"/{Command}@{botName}" : $"/{Command}");
    }

    public Task ExecuteAsync(Message message, string? payload) => ExecuteAsync(message, message.Chat, payload);

    public abstract Task ExecuteAsync(Message message, Chat chat, string? payload);

    internal string GetEscapedLine() => Utils.EscapeCharacters($"/{Command} – {Description}");

    protected readonly BotBase BotBase;
}