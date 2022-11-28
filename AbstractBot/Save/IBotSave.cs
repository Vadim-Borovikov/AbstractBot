using JetBrains.Annotations;

namespace AbstractBot.Save;

[PublicAPI]
public interface IBotSave<T> where T : new()
{
    public SaveComponent<T> SaveComponent { get; init; }
}