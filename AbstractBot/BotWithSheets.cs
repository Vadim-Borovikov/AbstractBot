using System;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public abstract class BotWithSheets<T> : Bot, IDisposable where T : ConfigGoogleSheets
{
    public new readonly T Config;

    protected readonly DocumentsManager DocumentsManager;

    protected BotWithSheets(T config) : base(config)
    {
        Config = config;
        DocumentsManager = new DocumentsManager(config);
    }

    public void Dispose() => DocumentsManager.Dispose();
}