using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles requests to the backend server's Requirements Bazaar portal
/// Important: This module does not directly communicate with the Requirements Bazaar
/// </summary>
public static class RequirementsBazaar
{
    /// <summary>
    /// Gets the available projects of the Requirements Bazaar
    /// </summary>
    /// <returns></returns>
    public static async Task<ApiResult<Project[]>> GetProjects()
    {
        Debug.Log("Requirements Baazar: GetProjects");
        Response resp = await Rest.GetAsync(
            ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects",
            null,
            -1,
            null,
            true);
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

    /// <summary>
    /// Gets the categories in a Requirements Bazaar Project
    /// </summary>
    /// <param name="projectId">The id of the project</param>
    /// <returns>The categories of the project, contained in the APIResult object</returns>
    public static async Task<ApiResult<Category[]>> GetCategoriesInProject(int projectId)
    {
        Response resp = await Rest.GetAsync(
            ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/categories",
            null,
            -1,
            null,
            true);
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

    /// <summary>
    /// Gets a Requirements Bazaar requirement by its id
    /// </summary>
    /// <param name="requirementId">The id of the requirement</param>
    /// <returns>The requirement as a general issue, contained in the APIResult object</returns>
    public static async Task<ApiResult<Issue>> GetRequirement(int requirementId)
    {
        Issue cached = IssueCache.GetRequirement(requirementId);
        if (cached != null)
        {
            return new ApiResult<Issue>(cached);
        }

        Response resp = await Rest.GetAsync(
            ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/requirements/" + requirementId,
            null,
            -1,
            null,
            true);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue issue = JsonUtility.FromJson<Issue>(resp.ResponseBody);
            IssueCache.AddIssue(issue);
            return new ApiResult<Issue>(issue);
        }
    }

    /// <summary>
    /// Gets the list of requirements in a Requirements Bazaar project
    /// </summary>
    /// <param name="projectId">The id of the project</param>
    /// <param name="page">Page of the content (the overall list is divided into pages)</param>
    /// <param name="itemsPerPage">Specifies how many items should be on one page</param>
    /// <returns>The list of requirements in the project on the given page, contained in the APIResult object</returns>
    public static async Task<ApiResult<Issue[]>> GetRequirementsInProject(int projectId, int page, int itemsPerPage)
    {
        return await GetRequirementsInProject(projectId, page, itemsPerPage, "");
    }

    /// <summary>
    /// Gets the list of requirements in a Requirements Bazaar project
    /// </summary>
    /// <param name="projectId">The id of the project</param>
    /// <param name="page">Page of the content (the overall list is divided into pages)</param>
    /// <param name="itemsPerPage">Specifies how many items should be on one page</param>
    /// <param name="search">Search pattern which filters the requirements</param>
    /// <returns>The list of requirements in the project on the given page, contained in the APIResult object</returns>
    public static async Task<ApiResult<Issue[]>> GetRequirementsInProject(int projectId, int page, int itemsPerPage, string search)
    {
        string path = ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
        search = StringUtilities.RemoveSpecialCharacters(search);
        if (!string.IsNullOrEmpty(search))
        {
            path += "&search=" + search;
        }
        Response resp = await Rest.GetAsync(path, null, -1, null, true);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue[] requirements = JsonArrayUtility.FromJson<Issue>(resp.ResponseBody);
            // add to cache
            foreach (Issue req in requirements)
            {
                IssueCache.AddIssue(req);
            }
            return new ApiResult<Issue[]>(requirements);
        }
    }

    /// <summary>
    /// Gets the requirements in a Requirements Bazaar Category
    /// </summary>
    /// <param name="categoryId">The id of the category</param>
    /// <param name="page">Page of the content (the overall list is divided into pages)</param>
    /// <param name="itemsPerPage">Specifies how many items should be on one page</param>
    /// <returns>The list of requirements in the category on hte given page, contained in the APIResult object</returns>
    public static async Task<ApiResult<Issue[]>> GetRequirementsInCategory(int categoryId, int page, int itemsPerPage)
    {
        return await GetRequirementsInCategory(categoryId, page, itemsPerPage, "");
    }

    /// <summary>
    /// Gets the requirements in a Requirements Bazaar Category
    /// </summary>
    /// <param name="categoryId">The id of the category</param>
    /// <param name="page">Page of the content (the overall list is divided into pages)</param>
    /// <param name="itemsPerPage">Specifies how many items should be on one page</param>
    /// <param name="search">Search pattern which filters the requirements</param>
    /// <returns>The list of requirements in the category on hte given page, contained in the APIResult object</returns>
    public static async Task<ApiResult<Issue[]>> GetRequirementsInCategory(int categoryId, int page, int itemsPerPage, string search)
    {
        string path = ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
        search = StringUtilities.RemoveSpecialCharacters(search);
        if (!string.IsNullOrEmpty(search))
        {
            path += "&search=" + search;
        }
        Response resp = await Rest.GetAsync(path, null, -1, null, true);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        if (!resp.Successful)
        {
            Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            return new ApiResult<Issue[]>(resp.ResponseCode, resp.ResponseBody);
        }
        else
        {
            Issue[] requirements = JsonArrayUtility.FromJson<Issue>(resp.ResponseBody);
            foreach(Issue req in requirements)
            {
                IssueCache.AddIssue(req);
            }
            return new ApiResult<Issue[]>(requirements);
        }
    }
}
