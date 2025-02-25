using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Modules.Servicies;
using AbstractBot.Models;
using AbstractBot.Models.Config;
using AbstractBot.Models.Operations.Commands;
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

namespace AbstractBot;

[PublicAPI]
public class BotCore : IDisposable
{
    public readonly TelegramBotClient Client;
    public readonly IConfig Config;
    public readonly Clock Clock;
    public readonly SerializerOptionsProvider JsonSerializerOptionsProvider;

    protected internal virtual KeyboardProvider? StartKeyboardProvider => null;

    public readonly User Self;

    public Logger Logger => _logging.Logger;

    public static async Task<BotCore?> TryCreateBotCore(Config config, CancellationToken cancellationToken = default)
    {
        TelegramBotClient client = new(config.Token);
        User self = await client.GetMe(cancellationToken);
        if (string.IsNullOrWhiteSpace(self.Username))
        {
            return null;
        }

        Clock clock = new(config.SystemTimeZoneId);
        SerializerOptionsProvider jsonSerializerOptionsProvider = new(clock);
        JsonSerializerOptions options = jsonSerializerOptionsProvider.SnakeCaseOptions;

        // string host = await GetHostAsync(options, Config.Host);

        string host =
            string.IsNullOrWhiteSpace(config.Host) ? await Manager.GetHostAsync(options) : config.Host;

        return new BotCore(config, client, self, host, clock);
    }

    protected BotCore(IConfig config, TelegramBotClient client, User self, string host, Clock clock)
    {
        Config = config;
        Client = client;
        Self = self;

        Clock = clock;

        JsonSerializerOptionsProvider = new SerializerOptionsProvider(Clock);

        _accesses = new Accesses(Config.Accesses.ToAccessDatasDictionary());

        TimeSpan tickInterval = TimeSpan.FromSeconds(Config.TickIntervalSeconds);
        _logging = new Logging(Clock, tickInterval);

        _connection = new ConnectionService(Client, host, Config.Token,
            TimeSpan.FromHours(Config.RestartPeriodHours), _logging.Logger);

        FileStorageService fileStorage = new();

        TimeSpan sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / Config.UpdatesPerSecondLimitPrivate);
        TimeSpan sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / Config.UpdatesPerSecondLimitGlobal);
        TimeSpan sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / Config.UpdatesPerMinuteLimitGroup);

        Cooldown cooldown = new(sendMessagePeriodPrivate, sendMessagePeriodGlobal, sendMessagePeriodGroup);

        UpdateSender = new UpdateSender(Client, fileStorage, cooldown, _logging);

        InputFileId dontUnderstandSticker = new(Config.DontUnderstandStickerFileId);
        InputFileId forbiddenSticker = new(Config.ForbiddenStickerFileId);

        _updateReceiver =
            new UpdateReceiver(dontUnderstandSticker, forbiddenSticker, Self.Id, UpdateSender, _logging);

        _commands = new Commands(Client, _accesses, _updateReceiver);

        string? selfUsername = Self.Username;
        if (selfUsername is null)
        {
            throw new Exception("Self username is null");
        }

        /*Greeter greeter = new(UpdateSender, Config.TextsBasic.StartFormat);

        Start start = new(_accesses, UpdateSender, _commands, Config.TextsBasic.StartCommandDescription,
            selfUsername, greeter);

        _updateReceiver.Operations.Add(start);*/

        Help help = new(_accesses, UpdateSender, _updateReceiver, Config.Texts.HelpCommandDescription, selfUsername,
            Config.Texts.HelpFormat);

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