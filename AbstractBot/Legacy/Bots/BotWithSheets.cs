using System;
using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Operations.Data;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public abstract class BotWithSheets<TConfig, TTexts, TContext, TContextData, TMetaContext, TSaveData, TStartData>
    : Bot<TConfig, TTexts, TContext, TContextData, TMetaContext, TSaveData, TStartData>, IDisposable
    where TConfig : ConfigWithSheets<TTexts>
    where TTexts : TextsBasic
    where TContext : class, IContext<TContext, TContextData, TMetaContext>
    where TContextData : class
    where TMetaContext : class
    where TSaveData : SaveData<TContextData>, new()
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