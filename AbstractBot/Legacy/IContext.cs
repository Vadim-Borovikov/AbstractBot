using JetBrains.Annotations;

namespace AbstractBot.Legacy;

[PublicAPI]
public interface IContext<out TContext, TContextData, in TMetaContext>
    where TContext : class, IContext<TContext, TContextData, TMetaContext>
    where TContextData : class
    where TMetaContext : class
{
    TContextData? Save();
    static abstract TContext? Load(TContextData data, TMetaContext? meta);
}