using System.Threading;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ICooldown
{
    void DelayIfNeeded(Chat chat, CancellationToken cancellationToken);
}