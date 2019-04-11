using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Core.Utilities.WebRequestRest;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class BackendConnector
{
    public static async Task<ApiResult<PunchCardEntry[]>> GetGitHubPunchCard(string owner, string repository)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "githubPunchCard/" + owner + "/" + repository);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<PunchCardEntry[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Debug.Log(resp.ResponseBody);
            PunchCardEntry[] gitHubPunchCard = JsonArrayUtility.FromJson<PunchCardEntry>(resp.ResponseBody);
            return new ApiResult<PunchCardEntry[]>(gitHubPunchCard);
        }
    }
}
