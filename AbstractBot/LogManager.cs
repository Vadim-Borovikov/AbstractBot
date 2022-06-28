using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public sealed class LogManager
{
    internal LogManager() => _timeManager = new TimeManager();

    public void SetTimeZone(string? timeZoneId = null) => _timeManager = new TimeManager(timeZoneId);

    public void LogMessage(string? message = null)
    {
        DateTime today = _timeManager.Now().Date;
        string path = string.Format(MessagesLogPathTemplete, $"{today:dd.MM.yyyy}");
        InsertToStart(path, $"{message}{Environment.NewLine}");
    }

    public void LogTimedMessage(string? message = null)
    {
        LogMessage($"{_timeManager.Now():HH:mm:ss}: {message}");
    }

    public void LogException(Exception ex)
    {
        LogTimedMessage($"Error: {ex.Message}");

        string message =
            $"{_timeManager.Now():dd.MM HH:mm:ss}{Environment.NewLine}{ex}{Environment.NewLine}{Environment.NewLine}";
        InsertToStart(ExceptionsLogPath, message);
    }

    public static void DeleteExceptionLog() => File.Delete(ExceptionsLogPath);

    internal void LogExceptionIfPresents(Task t)
    {
        if (t.Exception is null)
        {
            return;
        }

        LogException(t.Exception);
    }

    private static void InsertToStart(string path, string? contents)
    {
        string text = File.Exists(path) ? File.ReadAllText(path) : "";
        File.WriteAllText(path, $"{contents}{text}");
    }

    private TimeManager _timeManager;

    private const string ExceptionsLogPath = "errors.txt";
    private const string MessagesLogPathTemplete = "log {0}.txt";
}
