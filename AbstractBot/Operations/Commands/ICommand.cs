using Telegram.Bot.Types;
using static AbstractBot.Operations.OperationBasic;

namespace AbstractBot.Operations.Commands;

internal interface ICommand
{
    public BotCommand BotCommand { get; }
    public bool HideFromMenu { get; }
    public Access AccessLevel { get; }
}