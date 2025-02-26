using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Context;

[PublicAPI]
public interface IContext<out TContext, TContextData>
    where TContext : class, IContext<TContext, TContextData>
    where TContextData : class
{
    TContextData? Save();
    static abstract TContext? Load(TContextData data);
}