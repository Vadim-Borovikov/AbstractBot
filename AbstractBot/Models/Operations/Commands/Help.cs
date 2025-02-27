using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Models.MessageTemplates;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public sealed class Help : Command
{
    internal Help(IAccesses accesses, IUpdateSender updateSender, IUpdateReceiver updateReceiver, ITexts texts,
        string selfUsername)
        : base(accesses, updateSender, "help", texts, selfUsername)
    {
        _accesses = accesses;
        _updateReceiver = updateReceiver;
        _format = texts.HelpFormat;
    }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        AccessData access = _accesses.GetAccess(sender.Id);

        List<MessageTemplateText> descriptions =
            _updateReceiver.Operations
                           .Where(o => access.IsSufficientAgainst(o.AccessRequired))
                           .Select(o => o.HelpDescription)
                           .SkipNulls()
                           .ToList();

        MessageTemplateText template = MessageTemplateText.JoinTexts(descriptions);
        if (_format is not null)
        {
            template = _format.Format(template);
        }

        return template.SendAsync(UpdateSender, message.Chat);
    }

    private readonly IAccesses _accesses;
    private readonly IUpdateReceiver _updateReceiver;
    private readonly MessageTemplateText? _format;
}