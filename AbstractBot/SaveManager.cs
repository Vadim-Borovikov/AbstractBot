using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot;

[PublicAPI]
public class SaveManager<TData, TJsonData>
    where TData: class, new()
{
    public TData Data { get; private set; }

    public SaveManager(string path, Func<TJsonData?, TData?> fromJson, Func<TData?, TJsonData?> toJson)
    {
        _path = path;
        _fromJson = fromJson;
        _toJson = toJson;
        _locker = new object();
        Data = new TData();
    }

    public void Save()
    {
        lock (_locker)
        {
            TJsonData? jsonData = _toJson(Data);
            string json = jsonData is null ? "" : JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            File.WriteAllText(_path, json);
        }
    }

    public void Load()
    {
        lock (_locker)
        {
            if (!File.Exists(_path))
            {
                return;
            }
            string json = File.ReadAllText(_path);
            TJsonData? jsonData = JsonConvert.DeserializeObject<TJsonData>(json);
            Data = _fromJson(jsonData) ?? new TData();
        }
    }

    private readonly string _path;
    private readonly Func<TJsonData?, TData?> _fromJson;
    private readonly Func<TData?, TJsonData?> _toJson;
    private readonly object _locker;
}
