using AbstractBot.Legacy.Operations.Data;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;

namespace AbstractBot.Legacy.Operations.Commands;

[PublicAPI]
public abstract class CommandSimple : Command<CommandDataSimple>
{
    protected CommandSimple(MessageTemplateText descriptionFormat, string command, string description)
        : base(descriptionFormat, command, description)
    { }
}