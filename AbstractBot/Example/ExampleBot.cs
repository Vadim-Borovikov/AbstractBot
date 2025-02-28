using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.Config;
using AbstractBot.Models.Operations.Commands;
using AbstractBot.Models.Operations.Commands.Start;
using AbstractBot.Modules;
using AbstractBot.Modules.TextProviders;
using GryphonUtilities;

namespace AbstractBot.Example;

// ReSharper disable once UnusedType.Global
internal class ExampleBot : Bot
{
    public ExampleBot(IBotCore core, ExampleConfig config, ICommands commands, IStartCommand start, Help help)
        : base(core, commands, start, help)
    {
        _config = config;
        _saveManager = new SaveManager<ExampleSaveData>(config.SavePath, core.Clock, AfterLoad, BeforeSave);
    }

    private void AfterLoad()
    {
        List<int>? list = _saveManager.SaveData.List;
        _hashSet = list is null ? new HashSet<int>() : new HashSet<int>(list);
        _hashSet.Add(_config.SomeNumber);
    }

    private void BeforeSave() => _saveManager.SaveData.List = _hashSet.ToList();

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

        Common<Texts> common = new(config.MyTextsTemp);

        AccessBasedUserProvider userProvider = new(core.Accesses);

        ICommands commands = new Commands(core.Client, core.Accesses, core.UpdateReceiver, common, userProvider);

        Greeter greeter = new(core.UpdateSender, common.Texts.StartFormat);
        Start start = new(core.Accesses, core.UpdateSender, commands, common.GetDefaultTexts(), core.SelfUsername,
            greeter);

        Help help =
            new(core.Accesses, core.UpdateSender, core.UpdateReceiver, common.GetDefaultTexts(), core.SelfUsername);

        return new ExampleBot(core, config, commands, start, help);
    }

    private readonly ExampleConfig _config;
    private readonly SaveManager<ExampleSaveData> _saveManager;

    private HashSet<int> _hashSet = new();
}