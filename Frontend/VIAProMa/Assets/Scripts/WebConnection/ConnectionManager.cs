using HoloToolkit.Unity;
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
}
