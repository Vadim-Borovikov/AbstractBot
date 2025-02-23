using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces;

[PublicAPI]
public interface IFileStorage
{
    bool TryAdd(string path, FileBase file);
    InputFileId? TryGetInputFileId(string path);
}