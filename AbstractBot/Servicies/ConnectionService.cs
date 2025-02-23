using System;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Servicies;

[PublicAPI]
public sealed class ConnectionService : IConnection
{
    public string Host { get; }

    public ConnectionService(TelegramBotClient client, string host, string token, TimeSpan restartPeriod,
        Logger logger)
    {
        Host = host;

        _client = client;
        _url = $"{Host}/{token}";
        _restartPeriod = restartPeriod;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ConnectAsync(cancellationToken);

        Invoker.DoPeriodically(ReconnectAsync, _restartPeriod, false, _logger, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => _client.DeleteWebhook(false, cancellationToken);

    public Task ConnectAsync(CancellationToken cancellationToken) =>
        _client.SetWebhook(_url, allowedUpdates: Array.Empty<UpdateType>(), cancellationToken: cancellationToken);

    public async Task ReconnectAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogTimedMessage("Reconnecting to Telegram...");

        await _client.DeleteWebhook(false, cancellationToken);

        await ConnectAsync(cancellationToken);

        _logger.LogTimedMessage("...connected.");
    }

    private readonly string _url;
    private readonly TelegramBotClient _client;
    private readonly TimeSpan _restartPeriod;
    private readonly Logger _logger;
}