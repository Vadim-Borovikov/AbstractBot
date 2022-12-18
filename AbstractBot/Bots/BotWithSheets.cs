using System;
using AbstractBot.Configs;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class BotWithSheets<T> : CustomBot<T>, IDisposable where T : ConfigGoogleSheets
{
    protected readonly DocumentsManager DocumentsManager;

    protected BotWithSheets(T config) : base(config) => DocumentsManager = new DocumentsManager(Config);

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