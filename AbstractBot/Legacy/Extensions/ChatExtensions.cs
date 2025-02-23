using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Legacy.Extensions;

[PublicAPI]
public static class ChatExtensions
{
    public static bool IsGroup(this Chat chat) => chat.Type is ChatType.Group or ChatType.Supergroup;
}