using System;
using System.Linq;
using System.Threading.Tasks;
using AbstractBot.Ngrok;
using GoogleSheetsManager;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot;

[PublicAPI]
public static class Utils
{
    public static async Task<string> GetNgrokHost()
    {
        ListTunnelsResult listTunnels = await Provider.ListTunnels();
        string? url = listTunnels.Tunnels?.Where(t => t.Proto is DesiredNgrokProto).SingleOrDefault()?.PublicUrl;
        return url.GetValue("Can't retrieve NGrok host");
    }

    public static void DeleteExceptionLog() => System.IO.File.Delete(ExceptionsLogPath);

    public static Task LogExceptionAsync(Exception ex)
    {
        return System.IO.File.AppendAllTextAsync(ExceptionsLogPath, $"{ex}{Environment.NewLine}");
    }

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

    private const string ExceptionsLogPath = "errors.txt";
    private const string DesiredNgrokProto = "https";
}
