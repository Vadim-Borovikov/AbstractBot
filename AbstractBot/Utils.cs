using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Ngrok;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot;

[PublicAPI]
public static class Utils
{
    public static readonly ReplyKeyboardRemove NoKeyboard = new();

    public static readonly LogManager LogManager = new();

    public static bool IsGroup(Chat chat) => chat.Type is ChatType.Group or ChatType.Supergroup;

    public static void StartLogWith(string systemTimeZoneId)
    {
        LogManager.SetTimeZone(systemTimeZoneId);
        LogManager.LogMessage();
        LogManager.LogTimedMessage("Startup");
    }

    public static void FireAndForget(Func<CancellationToken, Task> doWork,
        CancellationToken cancellationToken = default)
    {
        Task.Run(() => doWork(cancellationToken), cancellationToken)
            .ContinueWith(LogManager.LogExceptionIfPresents, cancellationToken);
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

    internal static async Task<string> GetNgrokHostAsync(JsonSerializerOptions options)
    {
        ListTunnelsResult listTunnels = await Provider.ListTunnels(options);
        string? url = listTunnels.Tunnels?.Where(t => t?.Proto is DesiredNgrokProto).SingleOrDefault()?.PublicUrl;
        return url.GetValue("Can't retrieve NGrok host");
    }

    internal static TimeSpan? Max(TimeSpan? left, TimeSpan? right)
    {
        if (left is null)
        {
            return right;
        }

        return left.Value.CompareTo(right) switch
        {
            0  => left,
            1  => left,
            -1 => right,
            _  => throw new InvalidOperationException()
        };
    }

    private const string DesiredNgrokProto = "https";
}