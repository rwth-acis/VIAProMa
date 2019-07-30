using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// provides the necessary data for a connection to the backend
/// </summary>
public class ConnectionManager : Singleton<ConnectionManager>
{
    [Tooltip("The address of the backend, i.e. its url and port")]
    [SerializeField] private string backendAddress = "http://localhost:8080";

    [Tooltip("The base path of the backend's API.")]
    [SerializeField] private string basePath = "resources";

    /// <summary>
    /// The address of the backend, i.e. its url and port
    /// </summary>
    public string BackendAddress
    {
        get
        {
            return backendAddress;
        }
        set
        {
            backendAddress = value;
            TestConnection();
        }
    }

    /// <summary>
    /// Combination of hte backend address and the base path of the backend's API
    /// </summary>
    public string BackendAPIBaseURL
    {
        get
        {
            return backendAddress + "/" + basePath + "/";
        }
    }

    /// <summary>
    /// True if the backend server is reachable
    /// This value is updated automatically with every request which is made to the server
    /// </summary>
    /// <value></value>
    public bool BackendReachable
    {
        get;private set;
    }

    /// <summary>
    /// Checks the status code in order to determine if the backend is reachable
    /// </summary>
    /// <param name="code"></param>
    public void CheckStatusCode(long code)
    {
        if (code == 0) // device unavailable
        {
            SetBackendReachableStatus(false);
        }
        else
        {
            SetBackendReachableStatus(true);
        }
    }

    /// <summary>
    /// Sets the backend reachable status to the given value
    /// If the value is different from the previous state, the BackendOnlineStatusChanged event is invoked
    /// </summary>
    /// <param name="reachable"></param>
    private void SetBackendReachableStatus(bool reachable)
    {
        bool previousStatus = BackendReachable;
        BackendReachable = reachable;
        if (previousStatus != BackendReachable)
        {
            BackendOnlineStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private async void TestConnection()
    {
        await BackendConnector.Ping();
    }

    /// <summary>
    /// Raised if the reachability status of the backend is changed
    /// </summary>
    public event EventHandler BackendOnlineStatusChanged;
}
