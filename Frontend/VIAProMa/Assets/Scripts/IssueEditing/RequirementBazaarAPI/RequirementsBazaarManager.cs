using Microsoft.MixedReality.Toolkit.Utilities;
//using Org.Requirements_Bazaar.Common;
using Org.Requirements_Bazaar.DataModel;
using Org.Requirements_Bazaar.Managers;
using Org.Requirements_Bazaar.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.Login;
using System.Text;

namespace Org.Requirements_Bazaar.API
{

    public static class RequirementsBazaarManager
    {
        public static IRestConnector RestConnector = new UnityWebRequestRestConnector();
        public static IJsonSerializer JsonSerializer = new JsonUtilityAdapter();
        private const string baseUrl = "https://requirements-bazaar.org/bazaar/";

        /// <summary>
        /// Retrieves a particular project by its given id
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <returns>The project with the given Id.</returns>
        public static async Task<Project> GetProject(int projectId)
        {
            string url = baseUrl + "projects/" + projectId.ToString();

            WebResponse<string> response = await RestConnector.GetAsync(url, null);
            if (!response.Successful)
            {
                Debug.LogError(response.Content);
                return null;
            }
            else
            {
                Project project = JsonSerializer.FromJson<Project>(response.Content);
                return project;
            }
        }

        /// <summary>
        /// Retrieves the categories of a project
        /// </summary>
        /// <param name="projectId">The id of the project which contains the categories</param>
        /// <param name="page">The page number of the categories list</param>
        /// <param name="per_page">The number of categories on one page</param>
        /// <param name="searchFilter">A search query string</param>
        /// <param name="sortingMode">How the categories should be sorteds</param>
        /// <returns></returns>
        public static async Task<int[]> GetProjectCategories (int projectId, int page = 0, int per_page = 10, string searchFilter = "", ProjectSortingMode sortingMode = ProjectSortingMode.DEFAULT)
        {
            string url = baseUrl + "projects/" + projectId.ToString() + "/categories?page=" + page.ToString() + "&per_page=" + per_page.ToString();

            if (sortingMode != ProjectSortingMode.DEFAULT)
            {
                url += "&sort=" + sortingMode.ToString().ToLower();
            }
            if (searchFilter != "")
            {
                searchFilter = CleanString(searchFilter);
                url += "&search=" + searchFilter;
            }

            WebResponse<string> response = await RestConnector.GetAsync(url, null);
            if (!response.Successful)
            {
                Debug.LogError(response.Content);
                return null;
            }
            else
            {
                string json = JsonHelper.EncapsulateInWrapper(response.Content);
                int[] categoryList = JsonHelper.FromJson<int>(json);
                return categoryList;
            }
        }

        /// <summary>
        /// Retrieves the requirements of a project
        /// </summary>
        /// <param name="projectId">The id of the project which contains the requirements</param>
        /// <param name="page">The page number of the requirements list</param>
        /// <param name="per_page">The number of requirements on one page</param>
        /// <param name="searchFilter">A search query string</param>
        /// <param name="sortingMode">How the requirements should be sorteds</param>
        /// <returns></returns>
        public static async Task<Requirement[]> GetProjectRequirements (int projectId, int page = 0, int per_page = 10, string search = "", RequirementState filterState = RequirementState.ALL, RequirementsSortingMode sortMode = RequirementsSortingMode.DEFAULT)
        {
            string url = baseUrl + "projects/" + projectId.ToString() + "/requirements";
            url += "?state=" + filterState.ToString().ToLower();
            url += "&per_page=" + per_page.ToString();

            if (page > 0)
            {
                url += "&page=" + page.ToString();
            }
            if (search != "")
            {
                search = CleanString(search);
                url += "&search=" + search;
            }
            if (sortMode != RequirementsSortingMode.DEFAULT)
            {
                url += "&sort=" + sortMode.ToString().ToLower();
            }

            WebResponse<string> response = await RestConnector.GetAsync(url, null);
            if (!response.Successful)
            {
                Debug.LogError(response.Content);
                return null;
            }
            else
            {
                string json = JsonHelper.EncapsulateInWrapper(response.Content);
                Requirement[] requirements = JsonHelper.FromJson<Requirement>(json);
                return requirements;
            }
        }

        public static async Task<Requirement[]> GetAllProjectRequirements(int projectId, string search = "", RequirementState filterState = RequirementState.ALL, RequirementsSortingMode sortMode = RequirementsSortingMode.DEFAULT)
        {
            string url = baseUrl + "projects/" + projectId.ToString() + "/requirements";
            url += "?state=" + filterState.ToString().ToLower();

            if (search != "")
            {
                search = CleanString(search);
                url += "&search=" + search;
            }
            if (sortMode != RequirementsSortingMode.DEFAULT)
            {
                url += "&sort=" + sortMode.ToString().ToLower();
            }

            Response response = await Rest.GetAsync(url, null, -1, null, true);
            string responseBody = await response.GetResponseBody();
            if (!response.Successful)
            {
                Debug.LogError(responseBody);
                return null;
            }
            else
            {
                string json = JsonHelper.EncapsulateInWrapper(responseBody);
                Requirement[] requirements = JsonHelper.FromJson<Requirement>(json);
                return requirements;
            }
        }

        /// <summary>
        /// Gets a category by its ID
        /// </summary>
        /// <param name="categoryId">The ID of the category</param>
        /// <returns>The category</returns>
        public static async Task<int> GetCategory(int categoryId)
        {
            string url = baseUrl + "categories/" + categoryId.ToString();

            WebResponse<string> response = await RestConnector.GetAsync(url, null);

            if (!response.Successful)
            {
                //Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
                return await GetCategory(0);
               // return null;
            }
            else
            {
                int category = JsonUtility.FromJson<int>(response.Content);
                return category;
            }
        }

        /// <summary>
        /// Deletes a specific requirement by its id
        /// </summary>
        /// <param name="requirementId">The id of the requirement which should be deleted</param>
        /// <returns>The deleted requirement</returns>
        public static async Task<Requirement> DeleteRequirement (int requirementId)
        {
            string url = baseUrl + "requirements/" + requirementId.ToString();
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
                byte[] authentificationInfoInfoBytes = Encoding.UTF8.GetBytes(authentificationInfoInfo);
                string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                try
                {
                    WebResponse<string> resp = await RestConnector.DeleteAsync(url, headers);
                    if (!resp.Successful)
                    {
                        if (resp.Code == 401)
                        {
                            Debug.LogError("You are not authorized to delete this requirement.");
                        }
                        else
                        {
                            Debug.LogError(resp.Code + ": " + resp.Content);
                        }
                        return null;
                    }
                    else
                    {
                        Requirement requirement = JsonSerializer.FromJson<Requirement>(resp.Content);
                        return requirement;
                    }
                }
                catch (ArgumentNullException)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Deletes a specific requirement by its name
        /// </summary>
        /// <param name="requirementName">The name of the requirement which should be deleted</param>
        /// <param name="projectId">The id of the project of the requirement which should be deleted</param>
        /// <returns>The deleted requirement</returns>
        public static async Task<Requirement> DeleteRequirement(string requirementName, int projectId)
        {
            Requirement[] projectRequirements = await GetAllProjectRequirements(projectId);
            int requirementId = 0;
            for (int i = 0; i < projectRequirements.Length; i++)
            {
                if (projectRequirements[i].Name == requirementName)
                {
                    requirementId = projectRequirements[i].Id;
                    break;
                }

            }
            if(requirementId==0)
            {
                Debug.LogError("Requirement not found");
                return null;
            }
            string url = baseUrl + "requirements/" + requirementId.ToString();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            if(ServiceManager.GetService<LearningLayersOidcService>() != null)
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
                byte[] authentificationInfoInfoBytes = Encoding.UTF8.GetBytes(authentificationInfoInfo);
                string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                WebResponse<string> resp = await RestConnector.DeleteAsync(url, headers);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.Code + ": " + resp.Content);
                    return null;
                }
                else
                {
                    Requirement requirement = JsonSerializer.FromJson<Requirement>(resp.Content);
                    return requirement;
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

        /// <summary>
        /// Creates and posts a new requirement
        /// </summary>
        /// <param name="projectId">The id of the project where the requirement will be posted</param>
        /// <param name="name">The name/title of the requirement</param>
        /// <param name="description">The description of the requirement</param>
        /// <param name="categories">Categories in the project</param>
        /// <returns>The resulting requirement as it was saved on the server</returns>
        public static async Task<Requirement> CreateRequirement(int projectId, string name, string description, int[] categories = null)
        {
            string url = baseUrl + "requirements/";

            // check if categories were supplied; if no: look for the default category and it in there
            if (categories == null)
            {
                Project proj = await GetProject(projectId);
                categories = new int[1];
                categories[0] = await GetCategory(proj.DefaultCategoryId);
            }

            // convert the requirement to a uploadable format (the statistic fields of the requirement are not recognized as input by the service)
            UploadableRequirement toCreate = new UploadableRequirement(name, description, projectId, categories);

            string json = JsonUtility.ToJson(toCreate);

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
                byte[] authentificationInfoInfoBytes = Encoding.UTF8.GetBytes(authentificationInfoInfo);
                string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                WebResponse<string> resp = await RestConnector.PostAsync(url, json, headers);
                if (!resp.Successful)
                {
                    Debug.LogError(resp.Code + ": " + resp.Content);
                    return null;
                }
                else
                {
                    Requirement requirement = JsonSerializer.FromJson<Requirement>(resp.Content);
                    return requirement;
                }
            }
        }

        /// <summary>
        /// Edits a specific requirement by its id
        /// </summary>
        /// <param name="requirementName">The name of the requirement which should be deleted</param>
        /// <param name="projectId">The id of the project of the requirement which should be deleted</param>
        /// <param name="newName"> The updated title of the requirement</param>
        /// <param name="newDescription"> The updated description of the requirement</param>
        /// <returns>The deleted requirement</returns>
        public static async Task<Requirement> EditRequirement(int requirementId, int projectId, string newName, string newDescription)
        {
            Requirement[] projectRequirements = await GetProjectRequirements(projectId);
            Requirement requirement = null;
            for (int i = 0; i < projectRequirements.Length; i++)
            {
                if (projectRequirements[i].Id == requirementId)
                {
                    requirement = projectRequirements[i];
                    break;
                }

            }
            if (requirement == null)
            {
                Debug.LogError("Requirement not found");
                return null;
            }

            //Editing the requirement
            requirement.Name = newName;
            requirement.Description = newDescription;


            string url = baseUrl + "requirements";

            // convert the requirement to a uploadable format (the statistic fields of the requirement are not recognized as input by the service)
            UploadableRequirement uploadableRequirement = requirement.ToUploadFormat();

            string json = JsonUtility.ToJson(uploadableRequirement);

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
                byte[] authentificationInfoInfoBytes = Encoding.UTF8.GetBytes(authentificationInfoInfo);
                string encodedAuthentificationInfo = Convert.ToBase64String(authentificationInfoInfoBytes);

                headers.Add("Authorization", "Basic " + encodedAuthentificationInfo);
                headers.Add("access-token", ServiceManager.GetService<LearningLayersOidcService>().AccessToken);

                WebResponse<string> response = await RestConnector.PutAsync(url, json, headers);
                if (!response.Successful)
                {
                    Debug.LogError(response.Code + ": " + response.Content);
                    return null;
                }
                else
                {
                    Requirement updatedRequirement = JsonSerializer.FromJson<Requirement>(response.Content);
                    return updatedRequirement;
                }
            }
        }

        /// <summary>
        /// Updates the specified requirement
        /// Please first query for the requirement to change, then changing its parameters and then supply it to this function
        /// </summary>
        /// <param name="toUpdate">The requirement to update</param>
        /// <returns>The updated requirement</returns>
        public static async Task<Requirement> UpdateRequirement(Requirement toUpdate)
        {
            string url = baseUrl + "requirements";

            // convert the requirement to a uploadable format (the statistic fields of the requirement are not recognized as input by the service)
            UploadableRequirement uploadableRequirement = toUpdate.ToUploadFormat();

            string json = JsonUtility.ToJson(uploadableRequirement);

            WebResponse<string> response = await RestConnector.PutAsync(url, json);
            if (!response.Successful)
            {
                Debug.LogError(response.Code + ": " + response.Content);
                return null;
            }
            else
            {
                Requirement requirement = JsonSerializer.FromJson<Requirement>(response.Content);
                return requirement;
            }
        }

        /// <summary>
        /// Cleans a string of potenitally dangerous symbols, e.g. to avoid open redirection injection in parameters
        /// </summary>
        /// <returns></returns>
        private static string CleanString(string s)
        {
            s = s.Replace("/", "");
            s = s.Replace("\\", "");
            s = s.Replace(":", "");
            s = s.Replace("&", "");
            s = s.Replace("§", "");
            s = s.Replace("$", "");
            return s;
        }
    }

    public enum ProjectSortingMode
    {
        DEFAULT, NAME, LAST_ACTIVITY, REQUIREMENT, FOLLOWER
    }

    public enum RequirementsSortingMode
    {
        DEFAULT, DATE, LAST_ACTIVITY, NAME, VOTE, COMMENT, FOLLOWER, REALIZED
    }

    public enum RequirementState
    {
        ALL, OPEN, REALIZED
    }
}