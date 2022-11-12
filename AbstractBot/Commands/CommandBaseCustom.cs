using JetBrains.Annotations;

namespace AbstractBot.Commands;

[PublicAPI]
public abstract class CommandBaseCustom<TBot, TConfig> : CommandBase
    where TBot : BotBaseCustom<TConfig>
    where TConfig : Config
{
    protected CommandBaseCustom(TBot bot, string command, string description) : base(bot, command, description)
    {
        Bot = bot;
    }

    protected readonly TBot Bot;
}