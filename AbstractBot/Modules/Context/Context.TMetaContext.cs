using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context;

[PublicAPI]
public class Context<TContext, TContextData, TMetaContext>
    where TContext : class, IContext<TContext, TContextData, TMetaContext>
    where TContextData : class
{
    public Dictionary<long, TContext> Contexts { get; init; } = new();

    public Context(Dictionary<long, TContextData> save) => _save = save;

    public void Save() => _save.ReplaceWith(Contexts, c => c.Save());

    public void Load(TMetaContext meta) => Contexts.ReplaceWith(_save, d => TContext.Load(d, meta));

    private readonly Dictionary<long, TContextData> _save;
}