using System.IO;
using UnityEngine;

public static class FileLogger
{
    private static string logFilePath;

    static FileLogger()
    {
        // Determine the log file path relative to the build directory
        logFilePath = Path.Combine(Application.dataPath, "../debug/debug.log");

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
    }

    /// <summary>
    /// Logs a message to the debug log file.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogErrorToFile(string message)
    {
        Debug.Log("does this shit even work ");
        // Add a timestamp to each log entry
        string logEntry = $"[{System.DateTime.Now}] {message}\n";

        // Append the message to the log file
        File.AppendAllText(logFilePath, logEntry);
    }
}
