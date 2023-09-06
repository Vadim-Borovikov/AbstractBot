using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Configs;
using AbstractBot.Save;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations;

[PublicAPI]
public abstract class ContextOperation<TConfig, TTexts, TData, TContext> : Operation
    where TConfig : CustomConfig<TTexts>
    where TTexts : Texts
    where TData : Data, new()
    where TContext : Context
{
    protected ContextOperation(Bot<TConfig, TTexts, TData> bot) : base(bot) => _data = bot.SaveManager.Data;

    protected internal override Task<ExecutionResult> TryExecuteAsync(Message message, long senderId)
    {
        TContext? context = _data.GetContext<TContext>(senderId);
        if (context is null)
        {
            return Task.FromResult(ExecutionResult.UnsuitableOperation);
        }
        return TryExecuteAsync(message, senderId, context);
    }

    protected abstract Task<ExecutionResult> TryExecuteAsync(Message message, long senderId, TContext context);

    private readonly TData _data;
}