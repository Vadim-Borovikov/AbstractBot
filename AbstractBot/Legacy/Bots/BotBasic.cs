using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Extensions;
using AbstractBot.Legacy.Operations;
using AbstractBot.Legacy.Operations.Commands;
using AbstractBot.Models;
using AbstractBot.Modules;
using AbstractBot.Servicies;
using AbstractBot.Utilities.Extensions;
using AbstractBot.Utilities.Ngrok;
using GryphonUtilities;
using GryphonUtilities.Extensions;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public abstract class BotBasic : IDisposable
{
    public readonly TelegramBotClient Client;
    public readonly ConfigBasic ConfigBasic;
    public readonly Clock Clock;
    public readonly SerializerOptionsProvider JsonSerializerOptionsProvider;

    protected internal readonly List<OperationBasic> Operations;

    protected readonly InputFileId DontUnderstandSticker;
    protected readonly InputFileId ForbiddenSticker;

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

        Help help = new(ConfigBasic);
        Operations = new List<OperationBasic>
        {
            help
        };

        DontUnderstandSticker = new InputFileId(ConfigBasic.DontUnderstandStickerFileId);
        ForbiddenSticker = new InputFileId(ConfigBasic.ForbiddenStickerFileId);

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

        UpdatesSender = new UpdatesSender(Client, fileStorage, cooldown, _logging);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        Operations.Sort();

        await _connection.StartAsync(cancellationToken);

        await _logging.StartAsync(cancellationToken);

        await UpdateCommands(null, cancellationToken);
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
    public void AddOrUpdateAccesses(Dictionary<long, AccessData> toAdd) => _accesses.AddOrUpdate(toAdd);

    public void Update(Update update) => Invoker.FireAndForget(_ => UpdateAsync(update), _logging.Logger);

    public AccessData GetAccess(long userId) => _accesses.GetAccess(userId);

    protected internal Task UpdateCommandsFor(long userId, string? languageCode = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<ICommand> commands = GetMenuCommands();

        return Client.SetMyCommands(commands.Where(c => GetAccess(userId).IsSufficientAgainst(c.AccessRequired))
                                            .Select(ca => ca.BotCommand),
            BotCommandScope.Chat(userId), languageCode, cancellationToken);
    }

    protected virtual async Task UpdateAsync(Message message)
    {
        if (message.From is null)
        {
            throw new Exception("Message update with null From");
        }

        if (message.From.Id == Self.Id)
        {
            return;
        }

        await UpdateAsync(message, message.From);
    }

    protected virtual Task UpdateAsync(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Message is null)
        {
            throw new Exception("CallbackQuery update with null Message");
        }

        if (string.IsNullOrWhiteSpace(callbackQuery.Data))
        {
            throw new Exception("CallbackQuery update with null Data");
        }

        return UpdateAsync(callbackQuery.Message, callbackQuery.From, callbackQuery.Data);
    }

    protected virtual async Task<OperationBasic?> UpdateAsync(Message message, User sender,
        string? callbackQueryData = null)
    {
        if (string.IsNullOrWhiteSpace(callbackQueryData))
        {
            _logging.LogUpdate(message.Chat, Logging.UpdateType.ReceiveMessage, message.MessageId,
                $"{message.Text}{message.Caption}");
        }
        else
        {
            _logging.LogUpdate(message.Chat, Logging.UpdateType.ReceiveCallback, message.MessageId, callbackQueryData);
        }

        foreach (OperationBasic operation in Operations)
        {
            if (message.Chat.IsGroup() && !operation.EnabledInGroups)
            {
                continue;
            }

            if ((message.Chat.Type == ChatType.Channel) && !operation.EnabledInChannels)
            {
                continue;
            }

            OperationBasic.ExecutionResult result =
                await operation.TryExecuteAsync(this, message, sender, callbackQueryData);
            switch (result)
            {
                case OperationBasic.ExecutionResult.UnsuitableOperation: continue;
                case OperationBasic.ExecutionResult.AccessInsufficent:
                    await ProcessInsufficientAccess(message, sender, operation);
                    return operation;
                case OperationBasic.ExecutionResult.AccessExpired:
                    await ProcessExpiredAccess(message, sender, operation);
                    return operation;
                case OperationBasic.ExecutionResult.Success: return operation;
                default: throw new ArgumentOutOfRangeException(nameof(result));
            }
        }

        await ProcessUnclearOperation(message, sender);
        return null;
    }

    protected virtual Task UpdateAsync(PreCheckoutQuery _) => Task.CompletedTask;

    protected virtual Task ProcessUnclearOperation(Message message, User _)
    {
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return message.Chat.IsGroup()
            ? Task.CompletedTask
            : UpdatesSender.SendStickerAsync(message.Chat, DontUnderstandSticker, rp);
    }

    protected virtual Task ProcessInsufficientAccess(Message message, User _, OperationBasic __)
    {
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return message.Chat.IsGroup()
            ? Task.CompletedTask
            : UpdatesSender.SendStickerAsync(message.Chat, ForbiddenSticker, rp);
    }

    protected virtual Task ProcessExpiredAccess(Message message, User _, OperationBasic __)
    {
        return ProcessInsufficientAccess(message, _, __);
    }

    protected virtual KeyboardProvider GetDefaultKeyboardProvider(Chat _) => KeyboardProvider.Remove;

    private Task UpdateAsync(Update update)
    {
        return update.Type switch
        {
            UpdateType.Message => UpdateAsync(update.Message.Denull(nameof(update.Message))),
            UpdateType.CallbackQuery => UpdateAsync(update.CallbackQuery.Denull(nameof(update.CallbackQuery))),
            UpdateType.PreCheckoutQuery =>
                UpdateAsync(update.PreCheckoutQuery.Denull(nameof(update.PreCheckoutQuery))),
            _ => Task.CompletedTask
        };
    }

    private async Task UpdateCommands(string? languageCode, CancellationToken cancellationToken)
    {
        await Client.DeleteMyCommands(cancellationToken: cancellationToken);
        await Client.DeleteMyCommands(BotCommandScope.AllGroupChats(), cancellationToken: cancellationToken);
        await Client.DeleteMyCommands(BotCommandScope.AllChatAdministrators(),
            cancellationToken: cancellationToken);

        List<ICommand> commands = GetMenuCommands().ToList();
        await Client.SetMyCommands(
            commands.Where(c => AccessData.Default.IsSufficientAgainst(c.AccessRequired))
                    .Select(ca => ca.BotCommand),
            BotCommandScope.AllPrivateChats(), languageCode, cancellationToken);

        foreach (long userId in _accesses.Ids)
        {
            await Client.SetMyCommands(
                commands.Where(c => _accesses.GetAccess(userId).IsSufficientAgainst(c.AccessRequired))
                        .Select(ca => ca.BotCommand),
                BotCommandScope.Chat(userId), languageCode, cancellationToken);
        }
    }

    private IEnumerable<ICommand> GetMenuCommands() => Operations.OfType<ICommand>().Where(c => c.ShowInMenu);

    private readonly IConnection _connection;
    private readonly IAccesses _accesses;

    private readonly Logging _logging;

    public readonly IUpdatesSender UpdatesSender;
}