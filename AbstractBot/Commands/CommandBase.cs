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

    public virtual bool IsInvokingBy(string text, out string? payload, bool fromChat = false, string? botName = null)
    {
        if (!fromChat)
        {
            payload = Utils.GetPostfix(text, $"/{Command} ");
            if (!string.IsNullOrWhiteSpace(payload))
            {
                return true;
            }
        }

        payload = null;
        return (fromChat && (text == $"/{Command}@{botName}"))
               || (!fromChat && ((text == $"/{Command}") || (!string.IsNullOrWhiteSpace(Alias) && (text == Alias))));
    }

    public abstract Task ExecuteAsync(Message message, bool fromChat, string? payload);

    protected readonly BotBase BotBase;
}