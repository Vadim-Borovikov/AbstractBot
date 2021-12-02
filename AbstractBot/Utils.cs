using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Utils
    {
        public static Task<Message> FinalizeStatusMessageAsync(this ITelegramBotClient client, Message message,
            string postfix = "")
        {
            string text = $"_{message.Text}_ Готово\\.{postfix}";
            return client.EditMessageTextAsync(message.Chat, message.MessageId, text, ParseMode.MarkdownV2);
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
    }
}
