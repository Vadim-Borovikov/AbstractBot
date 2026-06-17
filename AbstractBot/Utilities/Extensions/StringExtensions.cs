using JetBrains.Annotations;

namespace AbstractBot.Utilities.Extensions;

[PublicAPI]
public static class StringExtensions
{
    public static string Escape(this string s) => s.Replace("_", "\\_")
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
}