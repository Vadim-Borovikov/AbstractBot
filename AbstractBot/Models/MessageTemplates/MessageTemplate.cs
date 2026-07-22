using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplate
{
    public readonly string Text;

    public KeyboardProvider? KeyboardProvider;
    public int? MessageThreadId;
    public bool DisableNotification;
    public bool ProtectContent;
    public ReplyParameters? ReplyParameters;
    public string? MessageEffectId;
    public string? BusinessConnectionId;
    public bool AllowPaidBroadcast;
    public long? DirectMessagesTopicId;
    public SuggestedPostParameters? SuggestedPostParameters;
    public long? ReceiverUserId;
    public string? CallbackQueryId;
    public CancellationToken CancellationToken;

    protected MessageTemplate(string text) => Text = text;
    protected MessageTemplate(MessageTemplate prototype) : this(prototype.Text)
    {
        KeyboardProvider = prototype.KeyboardProvider;
        MessageThreadId = prototype.MessageThreadId;
        DisableNotification = prototype.DisableNotification;
        ProtectContent = prototype.ProtectContent;
        ReplyParameters = prototype.ReplyParameters;
        CancellationToken = prototype.CancellationToken;
    }

    public override string ToString() => Text;

    public abstract Task<Message> SendAsync(IUpdateSender updateSender, Chat chat);
}