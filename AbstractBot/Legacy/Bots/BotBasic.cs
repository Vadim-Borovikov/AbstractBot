using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Servicies;
using AbstractBot.Legacy.Configs;
using AbstractBot.Models;
using AbstractBot.Models.Operations.Commands;
using AbstractBot.Models.Operations.Commands.Start;
using AbstractBot.Modules;
using AbstractBot.Modules.Servicies;
using AbstractBot.Utilities.Extensions;
using AbstractBot.Utilities.Ngrok;
using GryphonUtilities;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public abstract class BotBasic : IDisposable
{
    public readonly TelegramBotClient Client;
    public readonly ConfigBasic ConfigBasic;
    public readonly Clock Clock;
    public readonly SerializerOptionsProvider JsonSerializerOptionsProvider;

    protected internal virtual KeyboardProvider? StartKeyboardProvider => null;

    public readonly User Self;

    public Logger Logger => _logging.Logger;

    // before BotBasic creation
    private static async Task<string> GetHostAsync(JsonSerializerOptions options, string? defaultHost = null)
    {
        if (string.IsNullOrWhiteSpace(defaultHost))
        {
            return await Manager.GetHostAsync(options);
        }
        return defaultHost;
    }

    // Also:
    //  TelegramBotClient client = new(configBasic.Token);
    //  User self = await client.GetMe(cancellationToken);

    protected BotBasic(ConfigBasic configBasic, TelegramBotClient client, User self, string host)
    {
        ConfigBasic = configBasic;
        Client = client;
        Self = self;

        Clock = new Clock(ConfigBasic.SystemTimeZoneId);

        JsonSerializerOptionsProvider = new SerializerOptionsProvider(Clock);

        _accesses = new Accesses(ConfigBasic.Accesses.ToAccessDatasDictionary());

        TimeSpan tickInterval = TimeSpan.FromSeconds(configBasic.TickIntervalSeconds);
        _logging = new Logging(Clock, tickInterval);

        _connection = new ConnectionService(Client, host, configBasic.Token,
            TimeSpan.FromHours(configBasic.RestartPeriodHours), _logging.Logger);

        FileStorageService fileStorage = new();

        TimeSpan sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / configBasic.UpdatesPerSecondLimitPrivate);
        TimeSpan sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / configBasic.UpdatesPerSecondLimitGlobal);
        TimeSpan sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / configBasic.UpdatesPerMinuteLimitGroup);

        Cooldown cooldown = new(sendMessagePeriodPrivate, sendMessagePeriodGlobal, sendMessagePeriodGroup);

        UpdateSender = new UpdateSender(Client, fileStorage, cooldown, _logging);

        InputFileId dontUnderstandSticker = new(ConfigBasic.DontUnderstandStickerFileId);
        InputFileId forbiddenSticker = new(ConfigBasic.ForbiddenStickerFileId);

        _updateReceiver =
            new UpdateReceiver(dontUnderstandSticker, forbiddenSticker, Self.Id, UpdateSender, _logging);

        _commands = new Commands(Client, _accesses, _updateReceiver);

        string? selfUsername = Self.Username;
        if (selfUsername is null)
        {
            throw new Exception("Self username is null");
        }

        Greeter greeter = new(UpdateSender, configBasic.TextsBasic.StartFormat);

        Start start = new(_accesses, UpdateSender, _commands, configBasic.TextsBasic.StartCommandDescription,
            selfUsername, greeter);

        _updateReceiver.Operations.Add(start);

        Help help = new(_accesses, UpdateSender, _updateReceiver, configBasic.TextsBasic.HelpCommandDescription,
            selfUsername, configBasic.TextsBasic.HelpFormat);

        _updateReceiver.Operations.Add(help);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await _connection.StartAsync(cancellationToken);

        await _logging.StartAsync(cancellationToken);

        await _commands.UpdateCommands(cancellationToken);
    }

    public virtual Task StopAsync(CancellationToken cancellationToken) => _connection.StopAsync(cancellationToken);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _logging.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Update(Update update) => _updateReceiver.Update(update);

    private readonly IConnection _connection;
    private readonly IAccesses _accesses;

    private readonly Logging _logging;

    public readonly IUpdateSender UpdateSender;
    private readonly IUpdateReceiver _updateReceiver;
    private readonly ICommands _commands;
}