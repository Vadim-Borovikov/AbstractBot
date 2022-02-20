using System.IO;
using GoogleSheetsManager;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot;

[PublicAPI]
public class SaveManager<TData, TJsonData>
    where TData: class, IConvertableTo<TJsonData>, new()
    where TJsonData: IConvertableTo<TData>
{
    public TData Data { get; private set; }

    public SaveManager(string path)
    {
        _path = path;
        _locker = new object();
        Data = new TData();
    }

    public void Save()
    {
        lock (_locker)
        {
            TJsonData? jsonData = Data.Convert();
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
            Data = jsonData?.Convert() ?? new TData();
        }
    }

    private readonly string _path;
    private readonly object _locker;
}
