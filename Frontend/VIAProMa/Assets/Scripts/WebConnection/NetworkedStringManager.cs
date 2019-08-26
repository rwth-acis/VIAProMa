using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class NetworkedStringManager
{
    private const string serviceEndpoint = "networkedString";

    public static async Task<short> RegisterStringResource()
    {
        Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint);
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

    public static async void DeregisterStringResource(short id)
    {
        Response resp = await Rest.DeleteAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint + "/" + id);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseBody);
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

    public static async void SetString(short id, string text)
    {
        Response resp = await Rest.PutAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint + "/" + id, text);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseBody);
        }
    }
}
