using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities.Time;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Modules;

[PublicAPI]
public class Cooldown : ICooldown
{
    public Cooldown(TimeSpan sendMessagePeriodPrivate, TimeSpan sendMessagePeriodGlobal,
        TimeSpan sendMessagePeriodGroup)
    {
        _sendMessagePeriodPrivate = sendMessagePeriodPrivate;
        _sendMessagePeriodGlobal = sendMessagePeriodGlobal;
        _sendMessagePeriodGroup = sendMessagePeriodGroup;
    }

    public void DelayIfNeeded(Chat chat, CancellationToken cancellationToken)
    {
        lock (_locker)
        {
            DateTimeFull now = DateTimeFull.CreateUtcNow();

            TimeSpan? beforeGlobalUpdate = Clock.GetDelayUntil(_lastUpdateGlobal, _sendMessagePeriodGlobal, now);

            DateTimeFull? lastUpdateLocal = _lastUpdates.ContainsKey(chat.Id) ? _lastUpdates[chat.Id] : null;
            TimeSpan period = chat.Type == ChatType.Private ? _sendMessagePeriodPrivate : _sendMessagePeriodGroup;
            TimeSpan? beforeLocalUpdate = Clock.GetDelayUntil(lastUpdateLocal, period, now);

            TimeSpan? maxDelay = TimeSpanExtensions.Max(beforeGlobalUpdate, beforeLocalUpdate);
            if (maxDelay.HasValue)
            {
                Task.Delay(maxDelay.Value, cancellationToken).Wait(cancellationToken);
                now += maxDelay.Value;
            }

            _lastUpdateGlobal = now;
            _lastUpdates[chat.Id] = now;
        }
    }

    private readonly Dictionary<long, DateTimeFull> _lastUpdates = new();
    private readonly object _locker = new();

    private readonly TimeSpan _sendMessagePeriodPrivate;
    private readonly TimeSpan _sendMessagePeriodGlobal;
    private readonly TimeSpan _sendMessagePeriodGroup;

    private DateTimeFull? _lastUpdateGlobal;
}