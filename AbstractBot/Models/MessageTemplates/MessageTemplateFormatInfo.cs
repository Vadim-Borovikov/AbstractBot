using JetBrains.Annotations;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public sealed class MessageTemplateFormatInfo
{
    public readonly bool Escaped;
    public readonly string Text;

    public MessageTemplateFormatInfo(bool escaped, string text)
    {
        Escaped = escaped;
        Text = text;
    }
}