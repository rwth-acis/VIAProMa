using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class BackendConnector
{

    /// <summary>
    /// Method which checks whether the backend server is reachable
    /// </summary>
    /// <returns>returns true if the server is reachable, false otherwise</returns>
    public static async Task<bool> Ping()
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "ping");
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (resp.ResponseCode == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static async void SendLogs(string log)
    {
        Response resp = await Rest.PostAsync(ConnectionManager.Instance.BackendAPIBaseURL + "consoleLog", new UTF8Encoding().GetBytes(log));
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
    }

    public static async Task<ApiResult<PunchCardEntry[]>> GetGitHubPunchCard(string owner, string repository)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "githubPunchCard/" + owner + "/" + repository);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<PunchCardEntry[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            PunchCardEntry[] gitHubPunchCard = JsonArrayUtility.FromJson<PunchCardEntry>(resp.ResponseBody);
            return new ApiResult<PunchCardEntry[]>(gitHubPunchCard);
        }
    }

    /// <summary>
    /// Gets the saved projects and scene configurations
    /// </summary>
    /// <returns></returns>
    public static async Task<ApiResult<string[]>> GetProjects()
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "projects/");
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<string[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            string[] projects = JsonArrayUtility.FromJson<string>(resp.ResponseBody);
            return new ApiResult<string[]>(projects);
        }
    }
}
