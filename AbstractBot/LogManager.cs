using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public sealed class LogManager
{
    internal LogManager()
    {
        _timeManager = new TimeManager();
        if (!Directory.Exists(MessagesLogDirectory))
        {
            Directory.CreateDirectory(MessagesLogDirectory);
        }
        DeleteOldLogs();
    }

    public void SetTimeZone(string? timeZoneId = null) => _timeManager = new TimeManager(timeZoneId);

    public void LogMessage(string? message = null)
    {
        string name = GetLogNameFor(_timeManager.Now().Date);
        string path = Path.Combine(MessagesLogDirectory, name);
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

    private void DeleteOldLogs()
    {
        HashSet<string> newLogs = new();
        for (byte days = 0; days < LogsToHold; ++days)
        {
            DateTime date = _timeManager.Now().Date.AddDays(-days);
            string name = GetLogNameFor(date);
            newLogs.Add(name);
        }

        List<string> oldLogs =
            Directory.EnumerateFiles(MessagesLogDirectory).Where(f => !newLogs.Contains(f)).ToList();
        foreach (string log in oldLogs)
        {
            File.Delete(log);
        }
    }

    private static string GetLogNameFor(DateTime day) => string.Format(MessagesLogPathTemplete, $"{day:dd.MM.yyyy}");

    private TimeManager _timeManager;

    private const string ExceptionsLogPath = "errors.txt";
    private const string MessagesLogDirectory = "Logs";
    private const string MessagesLogPathTemplete = "log {0}.txt";
    private const byte LogsToHold = 5;
}
