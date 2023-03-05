using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.Login;
using i5.VIAProMa.Utilities;
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
        private const string baseUrl = "https://requirements-bazaar.org/bazaar/";

        /// <summary>
        /// Gets the available projects of the Requirements Bazaar
        /// </summary>
        /// <returns></returns>
        public static async Task<ApiResult<Project[]>> GetProjects()
        {
            Debug.Log("Requirements Baazar: GetProjects");
            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                Response resp = await Rest.GetAsync(
                    baseUrl + "projects/?state=all&per_page=500",
                    //ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects",
                    null,
                    -1,
                    null,
                    true);
                ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                string responseBody = await resp.GetResponseBody();
                Debug.Log(responseBody);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.ResponseCode + ": " + responseBody);
                    return new ApiResult<Project[]>(resp.ResponseCode, responseBody);
                }
                else
                {
                    responseBody = "{\"array\":" + responseBody + "}";
                    Project[] projects = JsonArrayUtility.FromJson<Project>(responseBody);
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
                        Response resp = await Rest.GetAsync(
                    //ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/?state=all&per_page=500",
                    baseUrl + "projects/?state=all&per_page=500",
                    headers,
                    -1,
                    null,
                    true);
                        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                        string responseBody = await resp.GetResponseBody();
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.ResponseCode + ": " + responseBody);
                            return new ApiResult<Project[]>(resp.ResponseCode, responseBody);
                        }
                        else
                        {
                            responseBody = "{\"array\":" + responseBody + "}";
                            Project[] projects = JsonArrayUtility.FromJson<Project>(responseBody);
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
                Response resp = await Rest.GetAsync(
                baseUrl + "projects/" + projectId + "/categories?state=all&per_page=500",
                //ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/categories",
                null,
                -1,
                null,
                true);
                ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                string responseBody = await resp.GetResponseBody();
                responseBody = "{\"array\":" + responseBody + "}";
                if (!resp.Successful)
                {
                    Debug.LogError(resp.ResponseCode + ": " + responseBody);
                    return new ApiResult<Category[]>(resp.ResponseCode, responseBody);
                }
                else
                {
                    Category[] categories = JsonArrayUtility.FromJson<Category>(responseBody);
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
                        Response resp = await Rest.GetAsync(
                //ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/categories",
                baseUrl + "projects/" + projectId + "/categories?state=all&per_page=500",
                headers,
                -1,
                null,
                true);
                        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                        string responseBody = await resp.GetResponseBody();
                        responseBody = "{\"array\":" + responseBody + "}";
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.ResponseCode + ": " + responseBody);
                            return new ApiResult<Category[]>(resp.ResponseCode, responseBody);
                        }
                        else
                        {
                            Category[] categories = JsonArrayUtility.FromJson<Category>(responseBody);
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

            Response resp = await Rest.GetAsync(
                baseUrl + "requirements/" + requirementId,
                //ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/requirements/" + requirementId,
                null,
                -1,
                null,
                true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            string responseBody = await resp.GetResponseBody();
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + responseBody);
                return new ApiResult<Issue>(resp.ResponseCode, responseBody);
            }
            else
            {
                Requirement req = JsonUtility.FromJson<Requirement>(responseBody);
                //req.Contributors = (await GetRequirementContributors(req.Id)).Value;
                Issue issue = Issue.fromRequirement(req);
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
        public static async Task<ApiResult<Issue[]>> GetRequirementsInProject(int projectId, int page = 0, int itemsPerPage = 10)
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
            //string path = ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/projects/" + projectId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
            string path = baseUrl + "projects/" + projectId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
            search = StringUtilities.RemoveSpecialCharacters(search);
            if (!string.IsNullOrEmpty(search))
            {
                path += "&search=" + search;
            }

            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                Response resp = await Rest.GetAsync(path, null, -1, null, true);
                ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                string responseBody = await resp.GetResponseBody();
                //responseBody = "{\"array\":" + responseBody + "}";
                if (!resp.Successful)
                {
                    Debug.LogError(resp.ResponseCode + ": " + responseBody);
                    return new ApiResult<Issue[]>(resp.ResponseCode, responseBody);
                }
                else
                {
                    Debug.Log(responseBody);
                    //Requirement[] requirements = JsonArrayUtility.FromJson<Requirement>(responseBody);
                    Requirement[] requirements = Newtonsoft.Json.JsonConvert.DeserializeObject<Requirement[]>(responseBody);
                    Debug.Log(responseBody);
                    Debug.Log(requirements);
                    foreach (Requirement req in requirements)
                    {
                        Debug.Log(req.Id);
                        //req.Contributors = (await GetRequirementContributors(req.Id)).Value;
                    }
                    Issue[] issues = Issue.fromRequirements(requirements);
                    // add to cache
                    foreach (Issue req in issues)
                    {
                        IssueCache.AddIssue(req);
                    }
                    return new ApiResult<Issue[]>(issues);
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
                        path = baseUrl + "projects/" + projectId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
                        Response resp = await Rest.GetAsync(path, headers, -1, null, true);
                        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                        string responseBody = await resp.GetResponseBody();
                        responseBody = "{\"array\":" + responseBody + "}";
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.ResponseCode + ": " + responseBody);
                            return new ApiResult<Issue[]>(resp.ResponseCode, responseBody);
                        }
                        else
                        {
                            Requirement[] requirements = JsonArrayUtility.FromJson<Requirement>(responseBody);
                            foreach (Requirement req in requirements)
                            {
                                //req.Contributors = (await GetRequirementContributors(req.Id)).Value;
                            }
                            Issue[] issues = Issue.fromRequirements(requirements);
                            // add to cache
                            foreach (Issue req in issues)
                            {
                                IssueCache.AddIssue(req);
                            }
                            return new ApiResult<Issue[]>(issues);
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
            //string path = ConnectionManager.Instance.BackendAPIBaseURL + "requirementsBazaar/categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
            string path = baseUrl + "categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
            search = StringUtilities.RemoveSpecialCharacters(search);
            if (!string.IsNullOrEmpty(search))
            {
                path += "&search=" + search;
            }

            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                Response resp = await Rest.GetAsync(path, null, -1, null, true);
                ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                string responseBody = await resp.GetResponseBody();
                responseBody = "{\"array\":" + responseBody + "}";
                if (!resp.Successful)
                {
                    Debug.LogError(resp.ResponseCode + ": " + responseBody);
                    return new ApiResult<Issue[]>(resp.ResponseCode, responseBody);
                }
                else
                {
                    Requirement[] requirements = JsonArrayUtility.FromJson<Requirement>(responseBody);
                    foreach (Requirement req in requirements)
                    {
                        //req.Contributors = (await GetRequirementContributors(req.Id)).Value;
                    }
                    Issue[] issues = Issue.fromRequirements(requirements);
                    // add to cache
                    foreach (Issue req in issues)
                    {
                        IssueCache.AddIssue(req);
                    }
                    return new ApiResult<Issue[]>(issues);
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
                        path = baseUrl + "categories/" + categoryId + "/requirements?page=" + page + "&per_page=" + itemsPerPage;
                        Response resp = await Rest.GetAsync(path, headers, -1, null, true);
                        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                        string responseBody = await resp.GetResponseBody();
                        responseBody = "{\"array\":" + responseBody + "}";
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.ResponseCode + ": " + responseBody);
                            return new ApiResult<Issue[]>(resp.ResponseCode, responseBody);
                        }
                        else
                        {
                            Requirement[] requirements = JsonArrayUtility.FromJson<Requirement>(responseBody);
                            foreach (Requirement req in requirements)
                            {
                                //req.Contributors = (await GetRequirementContributors(req.Id)).Value;
                            }
                            Issue[] issues = Issue.fromRequirements(requirements);
                            // add to cache
                            foreach (Issue req in issues)
                            {
                                IssueCache.AddIssue(req);
                            }
                            return new ApiResult<Issue[]>(issues);
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        return null;
                    }
                }
            }
        }

        public static async Task<ApiResult<Contributors>> GetRequirementContributors(int requirementId)
        {
            string path = baseUrl + "requirements/" + requirementId + "/contributors";

            if (ServiceManager.GetService<LearningLayersOidcService>().AccessToken == null || ServiceManager.GetService<LearningLayersOidcService>().AccessToken == "")
            {
                Response resp = await Rest.GetAsync(path, null, -1, null, true);
                ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                string responseBody = await resp.GetResponseBody();
                if (!resp.Successful)
                {
                    Debug.LogError(resp.ResponseCode + ": " + responseBody);
                    return new ApiResult<Contributors>(resp.ResponseCode, responseBody);
                }
                else
                {
                    Contributors contributors = JsonUtility.FromJson<Contributors>(responseBody);
                    return new ApiResult<Contributors>(contributors);
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
                        path = baseUrl + "requirements/" + requirementId + "/contributors";
                        Response resp = await Rest.GetAsync(path, headers, -1, null, true);
                        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
                        string responseBody = await resp.GetResponseBody();
                        if (!resp.Successful)
                        {
                            Debug.LogError(resp.ResponseCode + ": " + responseBody);
                            return new ApiResult<Contributors>(resp.ResponseCode, responseBody);
                        }
                        else
                        {
                            Contributors contributors = JsonUtility.FromJson<Contributors>(responseBody);
                            return new ApiResult<Contributors>(contributors);
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