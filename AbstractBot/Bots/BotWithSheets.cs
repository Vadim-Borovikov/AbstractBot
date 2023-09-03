using System;
using AbstractBot.Configs;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class BotWithSheets<TConfig, TTexts> : CustomBot<TConfig, TTexts>, IDisposable
    where TConfig : ConfigGoogleSheets<TTexts>
    where TTexts : Texts
{
    protected readonly DocumentsManager DocumentsManager;

    protected BotWithSheets(TConfig config) : base(config) => DocumentsManager = new DocumentsManager(Config);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DocumentsManager.Dispose();
        }
    }
}