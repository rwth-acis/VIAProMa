using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manager for sharing strings
/// The manager converts strings to numeric ids which can be sent over network
/// It also handles the conversion back from ids to strings.
/// </summary>
public static class NetworkedStringManager
{
    private const string serviceEndpoint = "networkedStrings";

    /// <summary>
    /// Converts a string to a networked id which can be sent in the sharing service
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <returns>The networked id</returns>
    public static async Task<short> StringToId(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return -1;
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "text/plain"); // overwrite the content type
        headers.Add("Accept", "text/plain"); // overwrite the accept type
        Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + serviceEndpoint, text, headers);
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

    /// <summary>
    /// Converts a text id to the corresponding string
    /// </summary>
    /// <param name="id">The id which identifies the text segment</param>
    /// <returns>The text segment of the id</returns>
    public static async Task<string> GetString(short id)
    {
        if (id == -1)
        {
            return "";
        }

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
