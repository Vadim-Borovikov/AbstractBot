using JetBrains.Annotations;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models;

[PublicAPI]
public class KeyboardProvider
{
    public ReplyMarkup? Keyboard;

    public KeyboardProvider(ReplyMarkup? keyboard) => Keyboard = keyboard;

    public static implicit operator KeyboardProvider(InlineKeyboardMarkup inline) => new(inline);
    public static implicit operator KeyboardProvider(ReplyKeyboardMarkup reply) => new(reply);

    public static KeyboardProvider Remove = new(new ReplyKeyboardRemove());
    public static KeyboardProvider Same = new(null);
}