using AbstractBot.Interfaces;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Modules.Servicies;
using AbstractBot.Modules;
using AbstractBot.Modules.Servicies;
using AbstractBot.Modules.Servicies.Logging;
using AbstractBot.Utilities.Extensions;
using AbstractBot.Utilities.Ngrok;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;
using JetBrains.Annotations;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot;

[PublicAPI]
public class BotCore : IBotCore, IDisposable
{
    public TelegramBotClient Client { get; }
    public Clock Clock { get; }
    public SerializerOptionsProvider JsonSerializerOptionsProvider { get; }

    public User Self { get; }
    public string SelfUsername { get; }

    public Chat ReportsDefault { get; }

    public IConnection Connection { get; }

    public Logging Logging { get; }
    ILogging IBotCore.Logging => Logging;

    public IUpdateSender UpdateSender { get; }
    public IUpdateReceiver UpdateReceiver { get; }

    public IAccesses Accesses { get; }

    public IConfig Config { get; }

    public BotCore(TelegramBotClient client, Clock clock, SerializerOptionsProvider jsonSerializerOptionsProvider,
        User self, string selfUsername, IConnection connection, IUpdateSender updateSender,
        IUpdateReceiver updateReceiver, IAccesses accesses, IConfig config, Chat reportsDefault, LoggerExtended logger)
    {
        Client = client;
        Clock = clock;
        JsonSerializerOptionsProvider = jsonSerializerOptionsProvider;
        Self = self;
        SelfUsername = selfUsername;
        Connection = connection;
        UpdateSender = updateSender;
        UpdateReceiver = updateReceiver;
        Accesses = accesses;
        Config = config;
        ReportsDefault = reportsDefault;

        TimeSpan tickInterval = TimeSpan.FromSeconds(config.TickIntervalSeconds);
        Logging = new Logging(logger, tickInterval);
    }

    public void Dispose() => Logging.Dispose();

    public static async Task<BotCore?> TryCreateAsync(IConfig config, CancellationToken cancellationToken)
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

        string host =
            string.IsNullOrWhiteSpace(config.Host) ? await Manager.GetHostAsync(options) : config.Host;

        Accesses accesses = new(config.Accesses.ToAccessDatasDictionary());

        Clock loggerClock = new(config.SystemTimeZoneIdLogs);
        LoggerExtended logger = new(loggerClock);
        Connection connection = new(client, host, config.Token, TimeSpan.FromHours(config.RestartPeriodHours), logger);

        FileStorageService fileStorage = new();

        TimeSpan sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitPrivate);
        TimeSpan sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitGlobal);
        TimeSpan sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / config.UpdatesPerMinuteLimitGroup);

        Cooldown cooldown = new(sendMessagePeriodPrivate, sendMessagePeriodGlobal, sendMessagePeriodGroup);
        Batcher batcher = new(config.MaxMessagesInBatch);

        UpdateSender updateSender = new(client, fileStorage, cooldown, batcher, logger);

        InputFileId dontUnderstandSticker = new(config.DontUnderstandStickerFileId);
        InputFileId forbiddenSticker = new(config.ForbiddenStickerFileId);

        Chat reportsDefault = new()
        {
            Id = config.ReportsDefaultChatId,
            Type = ChatType.Private
        };

        UpdateReceiver updateReceiver =
            new(dontUnderstandSticker, forbiddenSticker, self.Id, reportsDefault, updateSender, logger);

        return new BotCore(client, clock, jsonSerializerOptionsProvider, self, self.Username, connection, updateSender,
            updateReceiver, accesses, config, reportsDefault, logger);
    }
}