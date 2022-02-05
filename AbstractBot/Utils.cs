using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot;

[PublicAPI]
public static class Utils
{
    public static Task<Message> FinalizeStatusMessageAsync(this ITelegramBotClient client, Message message,
        string postfix = "")
    {
        string text = $"_{message.Text}_ Готово\\.{postfix}";
        return client.EditMessageTextAsync(message.Chat.Id, message.MessageId, text, ParseMode.MarkdownV2);
    }

    public static string EscapeCharacters(string s)
    {
        return s.Replace("_", "\\_")
                .Replace("*", "\\*")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("~", "\\~")
                .Replace("`", "\\`")
                .Replace(">", "\\>")
                .Replace("#", "\\#")
                .Replace("+", "\\+")
                .Replace("-", "\\-")
                .Replace("=", "\\=")
                .Replace("|", "\\|")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace(".", "\\.")
                .Replace("!", "\\!");
    }

    public static string? GetPostfix(string text, string prefix)
    {
        return text.StartsWith(prefix, StringComparison.Ordinal) ? text[prefix.Length..] : null;
    }
}