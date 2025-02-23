using JetBrains.Annotations;
using System.IO;
using Telegram.Bot.Types;

namespace AbstractBot.Utilities.Extensions;

[PublicAPI]
public static class FileStreamExtensions
{
    public static InputFileStream ToInputFileStream(this FileStream stream)
    {
        return new InputFileStream(stream, Path.GetFileName(stream.Name));
    }
}