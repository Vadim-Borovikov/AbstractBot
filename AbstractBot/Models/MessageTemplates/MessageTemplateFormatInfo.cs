using JetBrains.Annotations;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public sealed class MessageTemplateFormatInfo
{
    public readonly bool MarkdownV2;
    public readonly string Text;

    public MessageTemplateFormatInfo(bool markdownV2, string text)
    {
        MarkdownV2 = markdownV2;
        Text = text;
    }
}