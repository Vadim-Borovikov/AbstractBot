using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Context;

[PublicAPI]
public interface IContext<out TContext, TContextData, in TMetaContext>
    where TContext : class, IContext<TContext, TContextData, TMetaContext>
    where TContextData : class
{
    TContextData? Save();
    static abstract TContext? Load(TContextData data, TMetaContext meta);
}