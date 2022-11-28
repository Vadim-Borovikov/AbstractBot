using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Commands;

[PublicAPI]
public abstract class CommandBase : BotCommand
{
    protected virtual string? Alias => null;

    public virtual BotBase.AccessType Access => BotBase.AccessType.Users;

    protected CommandBase(BotBase bot, string command, string description)
    {
        BotBase = bot;
        Command = command;
        Description = description;
    }

    public bool IsInvokingBy(string? text, bool fromChat, string? botName, out string? payload)
    {
        if (!fromChat)
        {
            payload = text is null ? null : Utils.GetPostfix(text, $"/{Command} ");
            if (!string.IsNullOrWhiteSpace(payload))
            {
                return true;
            }
        }

        payload = null;

        return (!string.IsNullOrWhiteSpace(Alias) && (text == Alias))
               || (fromChat && (text == $"/{Command}@{botName}"))
               || (!fromChat && (text == $"/{Command}"));
    }

    public abstract Task ExecuteAsync(Message message, Chat chat, string? payload);

    internal string GetEscapedLine() => Utils.EscapeCharacters($"/{Command} – {Description}");

    protected readonly BotBase BotBase;
}