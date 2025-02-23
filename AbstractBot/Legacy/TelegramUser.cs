using System;
using GryphonUtilities.Helpers;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy;

[PublicAPI]
public class TelegramUser
{
    public readonly long Id;
    public readonly string? FirstName;
    public readonly string? LastName;
    public readonly string? Username;

    public readonly Uri? Uri;

    public string FullName => Text.AddWord(FirstName, LastName);
    public string LongDescriptor => Text.AddWord(FullName, Login);
    public string ShortDescriptor => Login ?? FullName;
    public string? Login => GetLogin(Username);

    public static Uri? GetUri(string? username)
    {
        return username is null ? null : new Uri(string.Format(UriFormat, username));
    }
    public static string? GetLogin(string? username) => username is null ? null : string.Format(LoginFormat, username);

    public static string? GetUsername(string? login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return null;
        }

        return login.StartsWith("@", StringComparison.Ordinal) ? login.Remove(0, 1) : null;
    }

    public TelegramUser(User user) : this(user.Id, user.FirstName, user.LastName, user.Username) { }
    public TelegramUser(Chat chat) : this(chat.Id, chat.FirstName, chat.LastName, chat.Username) { }

    private TelegramUser(long id, string? firstName, string? lastName, string? username)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Username = username;

        Uri = GetUri(Username);
    }

    private const string UriFormat = "https://t.me/{0}";
    private const string LoginFormat = "@{0}";
}