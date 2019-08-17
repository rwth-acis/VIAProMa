using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collects log outputs from Unity's console and sends them to the backend
/// </summary>
public class DiagnosticsSender : MonoBehaviour
{
    [Tooltip("Determines when logs are send.")]
    public DiagnosticSendMode sendMode = DiagnosticSendMode.ON_ERROR_WARNING;

    [Tooltip("If Send Mode is set to Interval, the value determines the interval in seconds when the logs should be sent.")]
    public int sendInterval = 10;

    /// <summary>
    /// Determines how many messages are at most stored and sent
    /// If set to 0, the buffer size is unlimited.
    /// Not used if sendMode is set to INTERVAL.
    /// </summary>
    [Tooltip("Determines how many messages are at most stored and sent. If set to 0, the buffer size is unlimited. Not used if Send Mode is Interval.")]
    public int maxMessageBufferSize = 10;

    private List<string> logs;

    /// <summary>
    /// Initializes the component
    /// </summary>
    private void Awake()
    {
        logs = new List<string>();
    }

    /// <summary>
    /// Initiates the sending routine if the DiagnosticSendMode is set to INTERVAL
    /// </summary>
    private void Start()
    {
        if (sendMode == DiagnosticSendMode.INTERVAL)
        {
            InvokeRepeating("SendLogs", 1f, sendInterval);
        }
    }

    /// <summary>
    /// Registers to receive console logs
    /// </summary>
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    /// <summary>
    /// Un-registers from console logs
    /// </summary>
    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Called if a new log was written to console
    /// Creates a log string and adds it to the internal log list
    /// </summary>
    /// <param name="logString">String which was written to console</param>
    /// <param name="stackTrace">The stack trace of the message</param>
    /// <param name="type">Type of the message</param>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logLine = CreateLogLine(logString, stackTrace, type);
        logs.Add(logLine);

        if (sendMode != DiagnosticSendMode.INTERVAL && maxMessageBufferSize > 0)
        {
            while(logs.Count > 0 && maxMessageBufferSize > logs.Count)
            {
                logs.RemoveAt(0);
            }
        }

        if ((type == LogType.Error || type == LogType.Exception) && (sendMode == DiagnosticSendMode.ON_ERROR || sendMode == DiagnosticSendMode.ON_ERROR_WARNING))
        {
            SendLogs();
        }
        else if (type == LogType.Warning && (sendMode == DiagnosticSendMode.ON_ERROR_WARNING))
        {
            SendLogs();
        }
    }

    /// <summary>
    /// Creates a string representation of a console message
    /// </summary>
    /// <param name="logString">String which was written to console</param>
    /// <param name="stackTrace">The stack trace of the message</param>
    /// <param name="type">The type of the message</param>
    /// <returns>A string representation for sending</returns>
    private static string CreateLogLine(string logString, string stackTrace, LogType type)
    {
        string logLine = "";
        if (type == LogType.Error || type == LogType.Exception)
        {
            logLine += "[!!!] ";
        }
        logLine += type.ToString() + ": ";
        logLine += logString;
        logLine += "\n" + stackTrace;
        logLine += "\n";
        return logLine;
    }

    /// <summary>
    /// Sends the collected logs to the backend.
    /// Also clears the logs once they have been sent.
    /// </summary>
    private void SendLogs()
    {
        if (logs.Count > 0)
        {
            string log = "";
            foreach (string logLine in logs)
            {
                log += logLine;
            }
            BackendConnector.SendLogs(log);
            logs.Clear();
        }
    }
}

/// <summary>
/// The send mode for the diagnostic system
/// </summary>
public enum DiagnosticSendMode
{
    /// <summary>
    /// Sending should only happen if an error was logged
    /// </summary>
    ON_ERROR,
    /// <summary>
    /// Sending should only happen if an error or warning was logged
    /// </summary>
    ON_ERROR_WARNING,
    /// <summary>
    /// Sending should happen frequently
    /// </summary>
    INTERVAL
}
