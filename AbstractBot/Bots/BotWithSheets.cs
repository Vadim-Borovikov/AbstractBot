using System;
using AbstractBot.Configs;
using AbstractBot.Operations.Data;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class BotWithSheets<TConfig, TTexts, TSaveData, TStartData>
    : Bot<TConfig, TTexts, TSaveData, TStartData>, IDisposable
    where TConfig : ConfigWithSheets<TTexts>
    where TTexts : TextsBasic
    where TSaveData : new()
    where TStartData : class, ICommandData<TStartData>
{
    protected readonly Manager DocumentsManager;

    protected BotWithSheets(TConfig config) : base(config) => DocumentsManager = new Manager(Config);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            DocumentsManager.Dispose();
        }

        _disposed = true;
    }

    private bool _disposed;
}