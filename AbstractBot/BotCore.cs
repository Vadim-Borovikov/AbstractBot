using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules.Servicies;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Models.Config;
using AbstractBot.Modules;
using AbstractBot.Modules.Servicies;
using AbstractBot.Utilities.Extensions;
using AbstractBot.Utilities.Ngrok;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using AbstractBot.Interfaces;

namespace AbstractBot;

[PublicAPI]
public class BotCore : IBotCore
{
    public TelegramBotClient Client { get; }
    public Clock Clock { get; }
    public SerializerOptionsProvider JsonSerializerOptionsProvider { get; }

    public User Self { get; }
    public string SelfUsername { get; }

    public IConnection Connection { get; }

    public ILogging Logging { get; }

    public IUpdateSender UpdateSender { get; }
    public IUpdateReceiver UpdateReceiver { get; }

    public IAccesses Accesses { get; }

    public Config Config { get; }
    public CancellationTokenSource CancellationSource { get; }

    public BotCore(TelegramBotClient client, Clock clock, SerializerOptionsProvider jsonSerializerOptionsProvider,
        User self, string selfUsername, IConnection connection, ILogging logging, IUpdateSender updateSender,
        IUpdateReceiver updateReceiver, IAccesses accesses, Config config, CancellationTokenSource cancellationSource)
    {
        Client = client;
        Clock = clock;
        JsonSerializerOptionsProvider = jsonSerializerOptionsProvider;
        Self = self;
        SelfUsername = selfUsername;
        Connection = connection;
        Logging = logging;
        UpdateSender = updateSender;
        UpdateReceiver = updateReceiver;
        Accesses = accesses;
        Config = config;
        CancellationSource = cancellationSource;
    }

    public static async Task<BotCore?> TryCreateAsync(Config config, CancellationTokenSource cancellationSource)
    {
        TelegramBotClient client = new(config.Token);
        User self = await client.GetMe(cancellationSource.Token);
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

        TimeSpan tickInterval = TimeSpan.FromSeconds(config.TickIntervalSeconds);
        Logging logging = new(clock, tickInterval, cancellationSource);

        Connection connection =
            new(client, host, config.Token, TimeSpan.FromHours(config.RestartPeriodHours),
                logging.Logger);

        FileStorageService fileStorage = new();

        TimeSpan sendMessagePeriodPrivate = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitPrivate);
        TimeSpan sendMessagePeriodGlobal = TimeSpan.FromSeconds(1.0 / config.UpdatesPerSecondLimitGlobal);
        TimeSpan sendMessagePeriodGroup = TimeSpan.FromMinutes(1.0 / config.UpdatesPerMinuteLimitGroup);

        Cooldown cooldown = new(sendMessagePeriodPrivate, sendMessagePeriodGlobal, sendMessagePeriodGroup);

        UpdateSender updateSender = new(client, fileStorage, cooldown, logging);

        InputFileId dontUnderstandSticker = new(config.DontUnderstandStickerFileId);
        InputFileId forbiddenSticker = new(config.ForbiddenStickerFileId);

        UpdateReceiver updateReceiver = new(dontUnderstandSticker, forbiddenSticker, self.Id, updateSender, logging);

        return new BotCore(client, clock, jsonSerializerOptionsProvider, self, self.Username, connection,
            logging, updateSender, updateReceiver, accesses, config, cancellationSource);
    }
}