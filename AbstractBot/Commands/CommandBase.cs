using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Commands;

[PublicAPI]
public abstract class CommandBase<TBot, TConfig> : BotCommand
    where TBot : BotBase<TBot, TConfig>
    where TConfig : Config
{
    protected virtual string? Alias => null;

    public virtual BotBase<TBot, TConfig>.AccessType Access => BotBase<TBot, TConfig>.AccessType.Users;

    protected CommandBase(TBot bot, string command, string description)
    {
        Bot = bot;
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

    protected readonly TBot Bot;
}