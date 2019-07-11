using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class RequirementsBazaar
{
    public static async Task<ApiResult<Project[]>> GetProjects()
    {
        Debug.Log("Requirements Baazar: GetProjects");
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects");
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
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
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/categories");
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
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

    public static async Task<ApiResult<Issue>> GetRequirement(int requirementId)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/requirements/" + requirementId);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue issue = JsonUtility.FromJson<Issue>(resp.ResponseBody);
            return new ApiResult<Issue>(issue);
        }
    }

    public static async Task<ApiResult<Contributors>> GetRequirementContributors(int requirementId)
    {
        Response resp = await Rest.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/requirements/" + requirementId + "/contributors");
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Contributors>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Contributors contributors = JsonUtility.FromJson<Contributors>(resp.ResponseBody);
            return new ApiResult<Contributors>(contributors);
        }
    }

    public static async Task<ApiResult<Issue[]>> GetRequirementsInProject(int projectId, int page, int itemsPerPage)
    {
        return await GetRequirementsInProject(projectId, page, itemsPerPage, "");
    }

    public static async Task<ApiResult<Issue[]>> GetRequirementsInProject(int projectId, int page, int itemsPerPage, string search)
    {
        string path = ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
        search = StringUtilities.RemoveSpecialCharacters(search);
        if (!string.IsNullOrEmpty(search))
        {
            path += "&search=" + search;
        }
        Response resp = await Rest.GetAsync(path);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue[] requirements = JsonArrayUtility.FromJson<Issue>(resp.ResponseBody);
            return new ApiResult<Issue[]>(requirements);
        }
    }

    public static async Task<ApiResult<Issue[]>> GetRequirementsInCategory(int categoryId, int page, int itemsPerPage)
    {
        return await GetRequirementsInCategory(categoryId, page, itemsPerPage, "");
    }

    public static async Task<ApiResult<Issue[]>> GetRequirementsInCategory(int categoryId, int page, int itemsPerPage, string search)
    {
        string path = ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
        search = StringUtilities.RemoveSpecialCharacters(search);
        if (!string.IsNullOrEmpty(search))
        {
            path += "&search=" + search;
        }
        Response resp = await Rest.GetAsync(path);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue[] requirements = JsonArrayUtility.FromJson<Issue>(resp.ResponseBody);
            return new ApiResult<Issue[]>(requirements);
        }
    }
}
