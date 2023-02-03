﻿using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.Login;
using i5.VIAProMa.Utilities;
using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using Org.Requirements_Bazaar.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.WebConnection
{
    /// <summary>
    /// Handles requests to the backend server's Requirements Bazaar portal
    /// Important: This module does not directly communicate with the Requirements Bazaar
    /// </summary>
    public static class RequirementsBazaar
    {

        public static IRestConnector RestConnector = new UnityWebRequestRestConnector();
        public static IJsonSerializer JsonSerializer = new JsonUtilityAdapter();
        /// <summary>
        /// Gets the available projects of the Requirements Bazaar
        /// </summary>
        /// <returns></returns>
        public static async Task<ApiResult<Project[]>> GetProjects()
        {
            RestConnector = new UnityWebRequestRestConnector();
            Debug.Log("Requirements Baazar: GetProjects");
            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects", null);
                ConnectionManager.Instance.CheckStatusCode(resp.Code);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.Code + ": " + resp.Content);
                    return new ApiResult<Project[]>(resp.Code, resp.Content);
                }
                else
                {
                    Project[] projects = Utilities.JsonArrayUtility.FromJson<Project>(resp.Content);
                    List<Project> visibleProjects = new List<Project>();
                    foreach (Project project in projects)
                    {
                        if (project.visibility == true)
                            visibleProjects.Add(project);
                    }
                    return new ApiResult<Project[]>(visibleProjects.ToArray());
                }
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (ServiceManager.GetService<LearningLayersOidcService>() != null)
                {
                    Debug.Log("Service not null");
                }

                // decode the access token
                string decodedToken = JWT_Decoding(ServiceManager.GetService<LearningLayersOidcService>().AccessToken);
                if (decodedToken == "")
                {
                    Debug.LogError("Invalid Access Token for Decoding.");
                    return null;
                }
                else
                {

                    // extract sub and preferred username information and encode for the header
                    string authentificationInfoInfo = GetBasicAuthentificationInfo(decodedToken);
                    byte[] authentificationInfoInfoBytes = System.Text.Encoding.UTF8.GetBytes(authentificationInfoInfo);
                    string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                    headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                    headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);
                    try
                    {
                        WebResponse<string> resp = await RestConnector.GetAsync("https://requirements-bazaar.org/bazaar/projects/?state=all&per_page=500", headers);
                        ConnectionManager.Instance.CheckStatusCode(resp.Code);
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.Code + ": " + resp.Content);
                            return new ApiResult<Project[]>(resp.Code, resp.Content);
                        }
                        else
                        {
                            string responseBody = "{\"array\":" + resp.Content + "}";
                            Project[] projects = Utilities.JsonArrayUtility.FromJson<Project>(responseBody);
                            return new ApiResult<Project[]>(projects);
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        Debug.LogError("ArgumentNullException");
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the categories in a Requirements Bazaar Project
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <returns>The categories of the project, contained in the APIResult object</returns>
        public static async Task<ApiResult<Category[]>> GetCategoriesInProject(int projectId)
        {
            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/categories", null);
                ConnectionManager.Instance.CheckStatusCode(resp.Code);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.Code + ": " + resp.Content);
                    return new ApiResult<Category[]>(resp.Code, resp.Content);
                }
                else
                {
                    Category[] categories = Utilities.JsonArrayUtility.FromJson<Category>(resp.Content);
                    return new ApiResult<Category[]>(categories);
                }
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (ServiceManager.GetService<LearningLayersOidcService>() != null)
                {
                    Debug.Log("Service not null");
                }

                // decode the access token
                string decodedToken = JWT_Decoding(ServiceManager.GetService<LearningLayersOidcService>().AccessToken);
                if (decodedToken == "")
                {
                    Debug.LogError("Invalid Access Token for Decoding.");
                    return null;
                }
                else
                {

                    // extract sub and preferred username information and encode for the header
                    string authentificationInfoInfo = GetBasicAuthentificationInfo(decodedToken);
                    byte[] authentificationInfoInfoBytes = System.Text.Encoding.UTF8.GetBytes(authentificationInfoInfo);
                    string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                    headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                    headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                    try
                    {
                        WebResponse<string> resp = await RestConnector.GetAsync("https://requirements-bazaar.org/bazaar/projects/" + projectId + "/categories?state=all&per_page=500", headers);
                        ConnectionManager.Instance.CheckStatusCode(resp.Code);
                        string responseBody = "{\"array\":" + resp.Content + "}";
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.Code + ": " + responseBody);
                            return new ApiResult<Category[]>(resp.Code, responseBody);
                        }
                        else
                        {
                            Category[] categories = Utilities.JsonArrayUtility.FromJson<Category>(responseBody);
                            return new ApiResult<Category[]>(categories);
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                }
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

            WebResponse<string> resp = await RestConnector.GetAsync(ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/requirements/" + requirementId, null);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            if (!resp.Successful)
            {
                Debug.LogError(resp.Code + ": " + resp.Content);
                return new ApiResult<Issue>(resp.Code, resp.Content);
            }
            else
            {
                Issue issue = JsonSerializer.FromJson<Issue>(resp.Content);
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

            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                WebResponse<string> resp = await RestConnector.GetAsync(path, null);
                ConnectionManager.Instance.CheckStatusCode(resp.Code);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.Code + ": " + resp.Content);
                    return new ApiResult<Issue[]>(resp.Code, resp.Content);
                }
                else
                {
                    Issue[] requirements = Utilities.JsonArrayUtility.FromJson<Issue>(resp.Content);
                    // add to cache
                    foreach (Issue req in requirements)
                    {
                        IssueCache.AddIssue(req);
                    }
                    return new ApiResult<Issue[]>(requirements);
                }
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (ServiceManager.GetService<LearningLayersOidcService>() != null)
                {
                    Debug.Log("Service not null");
                }

                // decode the access token
                string decodedToken = JWT_Decoding(ServiceManager.GetService<LearningLayersOidcService>().AccessToken);
                if (decodedToken == "")
                {
                    Debug.LogError("Invalid Access Token for Decoding.");
                    return null;
                }
                else
                {

                    // extract sub and preferred username information and encode for the header
                    string authentificationInfoInfo = GetBasicAuthentificationInfo(decodedToken);
                    byte[] authentificationInfoInfoBytes = System.Text.Encoding.UTF8.GetBytes(authentificationInfoInfo);
                    string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                    headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                    headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                    try
                    {
                        path = "https://requirements-bazaar.org/bazaar/projects/" + projectId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
                        WebResponse<string> resp = await RestConnector.GetAsync(path, headers);
                        ConnectionManager.Instance.CheckStatusCode(resp.Code);
                        string responseBody = "{\"array\":" + resp.Content + "}";
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.Code + ": " + responseBody);
                            return new ApiResult<Issue[]>(resp.Code, responseBody);
                        }
                        else
                        {
                            Issue[] requirements = Utilities.JsonArrayUtility.FromJson<Issue>(responseBody);
                            // add to cache
                            foreach (Issue req in requirements)
                            {
                                IssueCache.AddIssue(req);
                            }
                            return new ApiResult<Issue[]>(requirements);
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                }
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

            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                WebResponse<string> resp = await RestConnector.GetAsync(path, null);
                ConnectionManager.Instance.CheckStatusCode(resp.Code);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.Code + ": " + resp.Content);
                    return new ApiResult<Issue[]>(resp.Code, resp.Content);
                }
                else
                {
                    Issue[] requirements = Utilities.JsonArrayUtility.FromJson<Issue>(resp.Content);
                    foreach (Issue req in requirements)
                    {
                        IssueCache.AddIssue(req);
                    }
                    return new ApiResult<Issue[]>(requirements);
                }
            }
            else
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                if (ServiceManager.GetService<LearningLayersOidcService>() != null)
                {
                    Debug.Log("Service not null");
                }

                // decode the access token
                string decodedToken = JWT_Decoding(ServiceManager.GetService<LearningLayersOidcService>().AccessToken);
                if (decodedToken == "")
                {
                    Debug.LogError("Invalid Access Token for Decoding.");
                    return null;
                }
                else
                {

                    // extract sub and preferred username information and encode for the header
                    string authentificationInfoInfo = GetBasicAuthentificationInfo(decodedToken);
                    byte[] authentificationInfoInfoBytes = System.Text.Encoding.UTF8.GetBytes(authentificationInfoInfo);
                    string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                    headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                    headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                    try
                    {
                        path = "https://requirements-bazaar.org/bazaar/" + "categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
                        WebResponse<string> resp = await RestConnector.GetAsync(path, headers);
                        ConnectionManager.Instance.CheckStatusCode(resp.Code);
                        string responseBody = "{\"array\":" + resp.Content + "}";
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.Code + ": " + responseBody);
                            return new ApiResult<Issue[]>(resp.Code, responseBody);
                        }
                        else
                        {
                            Issue[] requirements = Utilities.JsonArrayUtility.FromJson<Issue>(responseBody);
                            foreach (Issue req in requirements)
                            {
                                IssueCache.AddIssue(req);
                            }
                            return new ApiResult<Issue[]>(requirements);
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Decodes the access token using JWT decoding
        /// </summary>
        /// <param name="accessToken">The access token of the user</param>
        /// <returns>The decoded access token</returns>
        private static string JWT_Decoding(string accessToken)
        {
            string[] tokenParts = accessToken.Split('.');
            if (tokenParts.Length > 2)
            {
                string decodedBody = tokenParts[1];
                int paddingLength = 4 - decodedBody.Length % 4;
                if (paddingLength < 4)
                {
                    decodedBody += new string('=', paddingLength);
                }
                byte[] bytes = System.Convert.FromBase64String(decodedBody);
                string decodedToken = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                return decodedToken;
            }
            return "";
        }

        /// <summary>
        /// Extracts the subject and preferred username information from the decoded access token
        /// </summary>
        /// <param name="decodedToken">The access token which has been decoded using JWT</param>
        /// <returns>A string containing the subject and preferred username</returns>
        private static string GetBasicAuthentificationInfo(string decodedToken)
        {
            string[] elements = decodedToken.Split(',');
            string sub = "";
            string preferred_username = "";

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].Contains("sub"))
                {
                    sub = elements[i].Split(':')[1];
                    sub = sub.Substring(1, sub.Length - 2);
                }
                if (elements[i].Contains("preferred_username"))
                {
                    preferred_username = elements[i].Split(':')[1];
                    preferred_username = preferred_username.Substring(1, preferred_username.Length - 2);
                }
            }

            return preferred_username + ":" + sub;

        }
    }
}