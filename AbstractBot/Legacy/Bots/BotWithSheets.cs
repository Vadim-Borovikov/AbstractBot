using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Operations.Data;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public abstract class BotWithSheets<TConfig, TTexts, TContext, TContextData, TMetaContext, TSaveData, TStartData>
    : Bot<TConfig, TTexts, TContext, TContextData, TMetaContext, TSaveData, TStartData>
    where TConfig : ConfigWithSheets<TTexts>
    where TTexts : TextsBasic
    where TContext : class, IContext<TContext, TContextData, TMetaContext>
    where TContextData : class
    where TMetaContext : class
    where TSaveData : SaveData<TContextData>, new()
    where TStartData : class, ICommandData<TStartData>
{
    protected readonly Manager DocumentsManager;

    protected BotWithSheets(TConfig config, TelegramBotClient client, User self, string host)
        : base(config, client, self, host)
    {
        DocumentsManager = new Manager(Config);
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        DocumentsManager.Dispose();
        base.Dispose(true);
    }
}