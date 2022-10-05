using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot;

[PublicAPI]
public class SaveManager<T>
    where T: new()
{
    public T Data { get; private set; }

    public SaveManager(string path)
    {
        _path = path;
        _locker = new object();
        Data = new T();
    }

    public void Save()
    {
        lock (_locker)
        {
            string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
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
            Data = JsonConvert.DeserializeObject<T>(json) ?? new T();
        }
    }

    private readonly string _path;
    private readonly object _locker;
}