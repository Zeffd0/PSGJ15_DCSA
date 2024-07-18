using UnityEngine;
using System;
using System.IO;

namespace DDtBJ.Scripts.Base
{
    public class ExceptionHandler : IndestructibleSingletonBehaviour<ExceptionHandler>
    {
        private string _logFilePath;

        private void Start()
        {
#if !UNITY_EDITOR
        // Set the log file path to a folder named "CrashLogs" in the same directory as the executable
        _logFilePath = Path.Combine(Application.dataPath, "../CrashLogs/crash_log.txt");
#endif
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
        Application.logMessageReceived += HandleException;
#endif
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
        Application.logMessageReceived -= HandleException;
#endif
        }

        private void HandleException(string logString, string stackTrace, LogType type)
        {
#if !UNITY_EDITOR
        if (type == LogType.Exception)
        {
            // Get the current timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Create the log message
            string logMessage = $"[{timestamp}] Exception:\n{logString}\n\nStack Trace:\n{stackTrace}\n\n";

            // Create the "CrashLogs" folder if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));

            // Append the log message to the log file
            File.AppendAllText(_logFilePath, logMessage);

            Debug.LogError("A null reference exception occurred. Exiting the application.");
            Application.Quit();
        }
#endif
        }
    }

}

