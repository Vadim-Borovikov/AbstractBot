using JetBrains.Annotations;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot;

[PublicAPI]
public class KeyboardProvider
{
    public IReplyMarkup? Keyboard;

    public KeyboardProvider(IReplyMarkup? keyboard) => Keyboard = keyboard;

    public static implicit operator KeyboardProvider(InlineKeyboardMarkup inline) => new(inline);
    public static implicit operator KeyboardProvider(ReplyKeyboardMarkup reply) => new(reply);

    public static KeyboardProvider Remove = new(new ReplyKeyboardRemove());
    public static KeyboardProvider Same = new(null);
}