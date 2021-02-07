using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SaveManager<TData>
        where TData: class, new()
    {
        public TData Data { get; private set; }

        public SaveManager(string path)
        {
            _path = path;
            _locker = new object();
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
                if (File.Exists(_path))
                {
                    string json = File.ReadAllText(_path);
                    Data = JsonConvert.DeserializeObject<TData>(json);
                }
            }

            if (Data == null)
            {
                Data = new TData();
            }
        }

        private readonly string _path;
        private readonly object _locker;
    }
}