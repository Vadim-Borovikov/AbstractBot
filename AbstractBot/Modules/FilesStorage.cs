using System.Collections.Generic;
using AbstractBot.Interfaces;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules;

[PublicAPI]
public class FileStorageService : IFileStorage
{
    public bool TryAdd(string path, FileBase file)
    {
        if (_cache.ContainsKey(path))
        {
            return false;
        }
        _cache[path] = file.FileId;
        return true;
    }

    public InputFileId? TryGetInputFileId(string path)
    {
        return _cache.ContainsKey(path) ? InputFile.FromFileId(_cache[path]) : null;
    }

    private readonly Dictionary<string, string> _cache = new();
}