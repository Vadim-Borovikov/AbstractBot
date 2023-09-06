using System;
using AbstractBot.Configs;
using AbstractBot.Save;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class BotWithSheets<TConfig, TTexts, TData> : Bot<TConfig, TTexts, TData>, IDisposable
    where TConfig : ConfigGoogleSheets<TTexts>
    where TTexts : Texts
    where TData : Data, new()
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