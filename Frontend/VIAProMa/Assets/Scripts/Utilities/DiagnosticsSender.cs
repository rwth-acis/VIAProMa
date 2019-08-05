using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagnosticsSender : MonoBehaviour
{
    public int sendInterval = 10;

    private List<string> logs;

    private void Awake()
    {
        logs = new List<string>();
    }

    private void Start()
    {
        InvokeRepeating("SendLogs", 1f, sendInterval);
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
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
        logs.Add(logLine);
    }

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
