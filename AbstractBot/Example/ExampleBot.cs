using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.Config;
using AbstractBot.Models.Operations.Commands;
using AbstractBot.Models.Operations.Commands.Start;
using AbstractBot.Modules;
using AbstractBot.Modules.Context.Localization;
using AbstractBot.Modules.TextProviders;
using GoogleSheetsManager.Documents;
using GryphonUtilities.Save;
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedType.Global

namespace AbstractBot.Example;

internal sealed class ExampleBot : Bot, IDisposable
{
    public ExampleBot(BotCore core, ICommands commands, IStartCommand start, Help help, ExampleConfig config,
        Dictionary<long, ExampleUserState> userStates)
        : base(core, commands, start, help)
    {
        _core = core;
        _sheetsManager = new Manager(config);

        _state = new ExampleBotState(userStates);
        _saveManager = new SaveManager<ExampleBotState, ExampleStateData>(config.SavePath, core.Clock);
    }

    public void Dispose()
    {
        _sheetsManager.Dispose();
        _core.Dispose();
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        _saveManager.LoadTo(_state);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _saveManager.Save(_state);
        await base.StopAsync(cancellationToken);
    }

    // ReSharper disable once UnusedMember.Global
    public static async Task<ExampleBot?> TryCreateAsync(ExampleConfig config, CancellationToken cancellationToken)
    {
        BotCore? core = await BotCore.TryCreateAsync(config, cancellationToken);
        if (core is null)
        {
            return null;
        }

        Dictionary<long, ExampleUserState> userStates = new();

        Localization<Texts, ExampleUserState, LocalizationUserStateData> localization =
            new(config.AllTexts, config.DefaultLanguageCode, userStates);

        LocalizationUserRegistrator<ExampleUserState, LocalizationUserStateData> registrator = new(userStates);

        ICommands commands = new Commands(core.Client, core.Accesses, core.UpdateReceiver, localization);

        Greeter greeter = new(core.UpdateSender, localization);
        Start start =
            new(core.Accesses, core.UpdateSender, commands, localization, core.SelfUsername, greeter, registrator);

        Help help = new(core.Accesses, core.UpdateSender, core.UpdateReceiver, localization, core.SelfUsername);

        return new ExampleBot(core, commands, start, help, config, userStates);
    }

    private readonly BotCore _core;
    private readonly SaveManager<ExampleBotState, ExampleStateData> _saveManager;
    private readonly Manager _sheetsManager;
    private readonly ExampleBotState _state;
}