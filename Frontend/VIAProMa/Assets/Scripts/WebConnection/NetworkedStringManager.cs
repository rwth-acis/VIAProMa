using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkedStringManager
{
    private const string serviceEndpoint = "networkedStrings";

    public static async Task<short> StringToId(string text)
    {
        Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint, text);
        if (resp.Successful)
        {
            return short.Parse(resp.ResponseBody);
        }
        else
        {
            Debug.LogError(resp.ResponseBody);
            return -1;
        }
    }

    public static async Task<string> GetString(short id)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint + "/" + id);
        if (resp.Successful)
        {
            return resp.ResponseBody;
        }
        else
        {
            Debug.LogError(resp.ResponseBody);
            return "";
        }
    }
}
