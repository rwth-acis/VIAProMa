using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : Singleton<ConnectionManager>
{
    [SerializeField] private string backendAddress = "http://localhost:8080";

    [SerializeField] private string basePath = "resources";

    public string BackendAddress
    {
        get
        {
            return backendAddress;
        }
    }

    public string BackendAPIBaseURL
    {
        get
        {
            return backendAddress + "/" + basePath + "/";
        }
    }
}
