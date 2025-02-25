using System;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations;

[PublicAPI]
public abstract class OperationBase : IOperation
{
    public virtual Enum? AccessRequired => null;

    public virtual bool EnabledInGroups => false;
    public virtual bool EnabledInChannels => false;

    public MessageTemplateText? HelpDescription { get; set; }

    protected OperationBase(IAccesses accesses, IUpdateSender updateSender)
    {
        _accesses = accesses;
        UpdateSender = updateSender;
    }

    public abstract Task<IOperation.ExecutionResult> TryExecuteAsync(Message message, User from,
        string? callbackQueryData);

    protected string? TryGetQueryCore(string query)
    {
        string typeName = GetType().Name;
        return query.StartsWith(typeName, StringComparison.InvariantCulture) ? query.Substring(typeName.Length) : null;
    }

    protected AccessData.Status CheckAccess(long userId) => _accesses.GetAccess(userId).CheckAgainst(AccessRequired);

    protected virtual Task ExecuteAsync(Message message, User from) => Task.CompletedTask;

    protected virtual Task ExecuteAsync(Message message, User from, string callbackQueryDataCore)
    {
        return Task.CompletedTask;
    }

    protected readonly IUpdateSender UpdateSender;

    private readonly IAccesses _accesses;
}