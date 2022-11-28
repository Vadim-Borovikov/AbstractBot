using GryphonUtilities;
using JetBrains.Annotations;

namespace AbstractBot.Save;

[PublicAPI]
public class SaveComponent<T> where T : new()
{
    public readonly SaveManager<T> SaveManager;

    public SaveComponent(IConfigSave config, TimeManager timeManager)
    {
        SaveManager = new SaveManager<T>(config.SavePath, timeManager);
    }
}