using System.Collections.Generic;
using AbstractBot.Interfaces.Modules.Context;
using AbstractBot.Utilities.Extensions;
using JetBrains.Annotations;

namespace AbstractBot.Modules.Context;

[PublicAPI]
public class Context<TContext, TContextData>
    where TContext : class, IContext<TContext, TContextData>
    where TContextData : class
{
    public Dictionary<long, TContext> Contexts { get; init; } = new();

    public Context(Dictionary<long, TContextData> save) => _save = save;

    public void Save() => _save.ReplaceWith(Contexts, c => c.Save());

    public void Load() => Contexts.ReplaceWith(_save, TContext.Load);

    private readonly Dictionary<long, TContextData> _save;
}