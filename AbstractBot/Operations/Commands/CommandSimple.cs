using AbstractBot.Configs.MessageTemplates;
using AbstractBot.Operations.Data;
using JetBrains.Annotations;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class CommandSimple : Command<CommandDataSimple>
{
    protected CommandSimple(MessageTemplateText descriptionFormat, string command, string description)
        : base(descriptionFormat, command, description)
    { }
}