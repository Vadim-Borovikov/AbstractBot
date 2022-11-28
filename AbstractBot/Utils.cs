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

namespace AbstractBot;

[PublicAPI]
public static class Utils
{
    public static readonly LogManager LogManager = new();

    public static Chat GetChat(long id, ChatType type = ChatType.Private)
    {
        return new Chat
        {
            Id = id,
            Type = type
        };
    }

    public static Chat GetChatWith(User user)
    {
        Chat result = GetChat(user.Id);
        result.FirstName = user.FirstName;
        result.LastName = user.LastName;
        result.Username = user.Username;
        return result;
    }

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

    public static string? GetPostfix(string text, string prefix)
    {
        return text.StartsWith(prefix, StringComparison.Ordinal) ? text[prefix.Length..] : null;
    }

    internal static async Task<string> GetNgrokHostAsync(JsonSerializerOptions options)
    {
        ListTunnelsResult listTunnels = await Provider.ListTunnels(options);
        string? url = listTunnels.Tunnels?.Where(t => t?.Proto is DesiredNgrokProto).SingleOrDefault()?.PublicUrl;
        return url.GetValue("Can't retrieve NGrok host");
    }

    internal static TimeSpan? Max(TimeSpan? a, TimeSpan? b)
    {
        if (a is null)
        {
            return b;
        }

        return a.Value.CompareTo(b) switch
        {
            0  => a,
            1  => a,
            -1 => b,
            _  => throw new InvalidOperationException()
        };
    }

    private const string DesiredNgrokProto = "https";
}