using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.Config;
using AbstractBot.Models.Operations.Commands;
using AbstractBot.Models.Operations.Commands.Start;
using AbstractBot.Modules;
using AbstractBot.Modules.Context.Localization;
using AbstractBot.Modules.TextProviders;
using GryphonUtilities.Save;

namespace AbstractBot.Example;

// ReSharper disable once UnusedType.Global
internal class ExampleBot : Bot
{
    public ExampleBot(IBotCore core, ExampleConfig config, SaveManager<ExampleFinalData, ExampleSaveData> saveManager,
        ICommands commands, IStartCommand start, Help help)
        : base(core, commands, start, help)
    {
        _config = config;
        _saveManager = saveManager;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        _saveManager.Load();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _saveManager.Save();
        await base.StopAsync(cancellationToken);
    }

    // ReSharper disable once UnusedMember.Global
    public static async Task<ExampleBot?> TryCreateAsync(ExampleConfig config,
        CancellationTokenSource cancellationSource)
    {
        BotCore? core = await BotCore.TryCreateAsync(config, cancellationSource);
        if (core is null)
        {
            return null;
        }

        SaveManager<ExampleFinalData, ExampleSaveData> saveManager = new(config.SavePath, core.Clock);

        Localization<Texts, ExampleSaveData, ExampleUserFinalData, LocalizationUserSaveData> localization =
            new(config.AllTexts, config.DefaultLanguageCode, saveManager.FinalData);

        AccessBasedUserProvider userProvider = new(core.Accesses);

        ICommands commands = new Commands(core.Client, core.Accesses, core.UpdateReceiver, localization, userProvider);

        Texts defaultTexts = localization.GetDefaultTexts();
        Greeter greeter = new(core.UpdateSender, defaultTexts.StartFormat);
        Start start = new(core.Accesses, core.UpdateSender, commands, defaultTexts, core.SelfUsername, greeter);

        Help help = new(core.Accesses, core.UpdateSender, core.UpdateReceiver, defaultTexts, core.SelfUsername);

        return new ExampleBot(core, config, saveManager, commands, start, help);
    }

    private readonly ExampleConfig _config;
    private readonly SaveManager<ExampleFinalData, ExampleSaveData> _saveManager;
}