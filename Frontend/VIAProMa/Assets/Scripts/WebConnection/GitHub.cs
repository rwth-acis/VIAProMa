using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class GitHub
{
    public static async Task<ApiResult<Issue[]>> GetIssuesInRepository(string owner, string repositoryName)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "gitHub/repos/" + owner + "/" + repositoryName + "/issues");
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue[] issues = JsonArrayUtility.FromJson<Issue>(resp.ResponseBody);
            return new ApiResult<Issue[]>(issues);
        }
    }
}
