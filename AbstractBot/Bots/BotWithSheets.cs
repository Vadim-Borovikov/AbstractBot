using System;
using AbstractBot.Configs;
using AbstractBot.Operations.Infos;
using GoogleSheetsManager.Documents;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class BotWithSheets<TConfig, TTexts, TData, TStartInfo>
    : Bot<TConfig, TTexts, TData, TStartInfo>, IDisposable
    where TConfig : ConfigWithSheets<TTexts>
    where TTexts : Texts
    where TData : SaveData, new()
    where TStartInfo : class, ICommandInfo<TStartInfo>
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