using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class RequirementsBazaar
{
    public static async Task<ApiResult<Project[]>> GetProjects()
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "projects/");
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Project[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Project[] projects = JsonArrayUtility.FromJson<Project>(resp.ResponseBody);
            return new ApiResult<Project[]>(projects);
        }
    }

    public static async Task<ApiResult<Category[]>> GetCategoriesInProject(int projectId)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "projects/" + projectId + "/categories");
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Category[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Category[] categories = JsonArrayUtility.FromJson<Category>(resp.ResponseBody);
            return new ApiResult<Category[]>(categories);
        }
    }
}
