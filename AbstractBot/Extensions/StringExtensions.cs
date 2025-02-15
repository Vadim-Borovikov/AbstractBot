using JetBrains.Annotations;

namespace AbstractBot.Extensions;

[PublicAPI]
public static class StringExtensions
{
    public static string Escape(this string s, bool withCurlies = true)
    {
        string result = s.Replace("_", "\\_")
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
                         .Replace(".", "\\.")
                         .Replace("!", "\\!");
        if (withCurlies)
        {
            result = result.Replace("{", "\\{")
                           .Replace("}", "\\}");
        }

        return result;
    }
}