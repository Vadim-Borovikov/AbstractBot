using AbstractBot.Interfaces.Modules.Servicies;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces;

[PublicAPI]
public interface IBotCore
{
    public TelegramBotClient Client { get; }
    public Clock Clock { get; }
    public SerializerOptionsProvider JsonSerializerOptionsProvider { get; }

    public User Self { get; }
    public string SelfUsername { get; }

    public Chat ReportsDefault { get; }

    public IConnection Connection { get; }

    public ILogging Logging { get; }

    public IUpdateSender UpdateSender { get; }
    public IUpdateReceiver UpdateReceiver { get; }

    public IAccesses Accesses { get; }

    public IConfig Config { get; }
}