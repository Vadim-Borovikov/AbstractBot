﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AbstractBot.Ngrok;
using GryphonUtilities;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public static class Utils
{
    public static async Task<string> GetNgrokHost()
    {
        ListTunnelsResult listTunnels = await Provider.ListTunnels();
        string? url = listTunnels.Tunnels?.Where(t => t?.Proto is DesiredNgrokProto).SingleOrDefault()?.PublicUrl;
        return url.GetValue("Can't retrieve NGrok host");
    }

    public static void DeleteExceptionLog() => System.IO.File.Delete(ExceptionsLogPath);

    public static Task LogExceptionAsync(Exception ex)
    {
        return System.IO.File.AppendAllTextAsync(ExceptionsLogPath, $"{ex}{Environment.NewLine}");
    }

    public static Task ContinueWithHandling(this Task task)
    {
        return task.ContinueWith(t => t.Exception is null ? Task.CompletedTask : LogExceptionAsync(t.Exception));
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

    private const string ExceptionsLogPath = "errors.txt";
    private const string DesiredNgrokProto = "https";
}