using System.Threading;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces;

[PublicAPI]
public interface ICooldown
{
    void DelayIfNeeded(Chat chat, CancellationToken cancellationToken);
}