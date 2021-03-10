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

namespace Org.Requirements_Bazaar.API
{

    public static class RequirementsBazaarManager
    {
        private const string baseUrl = "https://requirements-bazaar.org/bazaar/";

        /// <summary>
        /// Retrieves a particular project by its given id
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <returns>The project with the given Id.</returns>
        public static async Task<Project> GetProject(int projectId)
        {
            string url = baseUrl + "projects/" + projectId.ToString();

            Response response = await Rest.GetAsync(url, null, -1, null, true);
            if (!response.Successful)
            {
                Debug.LogError(response.ResponseBody);
                return null;
            }
            else
            {
                Project project = JsonUtility.FromJson<Project>(response.ResponseBody);
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
        public static async Task<Category[]> GetProjectCategories (int projectId, int page = 0, int per_page = 10, string searchFilter = "", ProjectSortingMode sortingMode = ProjectSortingMode.DEFAULT)
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

            Response response = await Rest.GetAsync(url, null, -1, null, true);
            if (!response.Successful)
            {
                Debug.LogError(response.ResponseBody);
                return null;
            }
            else
            {
                string json = JsonHelper.EncapsulateInWrapper(response.ResponseBody);
                Category[] categoryList = JsonHelper.FromJson<Category>(json);
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

            Response response = await Rest.GetAsync(url, null, -1, null, true);
            if (!response.Successful)
            {
                Debug.LogError(response.ResponseBody);
                return null;
            }
            else
            {
                string json = JsonHelper.EncapsulateInWrapper(response.ResponseBody);
                Requirement[] requirements = JsonHelper.FromJson<Requirement>(json);
                return requirements;
            }
        }

        /// <summary>
        /// Gets a category by its ID
        /// </summary>
        /// <param name="categoryId">The ID of the category</param>
        /// <returns>The category</returns>
        public static async Task<Category> GetCategory(int categoryId)
        {
            string url = baseUrl + "categories/" + categoryId.ToString();

            Response response = await Rest.GetAsync(url, null, -1, null, true);

            if (!response.Successful)
            {
                Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
                return null;
            }
            else
            {
                Category category = JsonUtility.FromJson<Category>(response.ResponseBody);
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
            Response resp = await Rest.DeleteAsync(url, null, -1, true);
            if(!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return null;
            }
            else
            {
                Requirement requirement = JsonUtility.FromJson<Requirement>(resp.ResponseBody);
                return requirement;
            }
        }

        /// <summary>
        /// Deletes a specific requirement by its id
        /// </summary>
        /// <param name="requirementName">The name of the requirement which should be deleted</param>
        /// /// <param name="projectId">The id of the project of the requirement which should be deleted</param>
        /// <returns>The deleted requirement</returns>
        public static async Task<Requirement> DeleteRequirement(string requirementName, int projectId)
        {
            Requirement[] projectRequirements = await GetProjectRequirements(projectId);
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
            Debug.Log(requirementId);
            string url = baseUrl + "requirements/" + requirementId.ToString();
            Response resp = await Rest.DeleteAsync(url, null,-1, true);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return null;
            }
            else
            {
                Requirement requirement = JsonUtility.FromJson<Requirement>(resp.ResponseBody);
                return requirement;
            }
        }

        /// <summary>
        /// Creates and posts a new requirement
        /// </summary>
        /// <param name="projectId">The id of the project where the requirement will be posted</param>
        /// <param name="name">The name/title of the requirement</param>
        /// <param name="description">The description of the requirement</param>
        /// <param name="categories">Categories in the project</param>
        /// <returns>The resulting requirement as it was saved on the server</returns>
        public static async Task<Requirement> CreateRequirement(int projectId, string name, string description, Category[] categories = null)
        {
            string url = baseUrl + "requirements/";

            // check if categories were supplied; if no: look for the default category and it in there
            if (categories == null)
            {
                Project proj = await GetProject(projectId);
                categories = new Category[1];
                categories[0] = await GetCategory(proj.DefaultCategoryId);
            }

            // convert the requirement to a uploadable format (the statistic fields of the requirement are not recognized as input by the service)
            UploadableRequirement toCreate = new UploadableRequirement(name, description, projectId, categories);

            string json = JsonUtility.ToJson(toCreate);

            Response resp = await Rest.PostAsync(url, json, null, -1, true);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return null;
            }
            else
            {
                Requirement requirement = JsonUtility.FromJson<Requirement>(resp.ResponseBody);
                return requirement;
            }
        }

        /// <summary>
        /// Edits a specific requirement by its id
        /// </summary>
        /// <param name="requirementName">The name of the requirement which should be deleted</param>
        /// /// <param name="projectId">The id of the project of the requirement which should be deleted</param>
        /// <returns>The deleted requirement</returns>
        public static async Task<Requirement> EditRequirement(string requirementName, int projectId)
        {
            Requirement[] projectRequirements = await GetProjectRequirements(projectId);
            Requirement requirement = null;
            for (int i = 0; i < projectRequirements.Length; i++)
            {
                if (projectRequirements[i].Name == requirementName)
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
            Debug.Log(requirement.Id);

            //Editing the requirement
            requirement.Name = "Updated Requirement";


            string url = baseUrl + "requirements/" + requirement.Id;

            // convert the requirement to a uploadable format (the statistic fields of the requirement are not recognized as input by the service)
            UploadableRequirement uploadableRequirement = requirement.ToUploadFormat();

            string json = JsonUtility.ToJson(uploadableRequirement);

            Response response = await Rest.PutAsync(url, json, null, -1, true);
            if (!response.Successful)
            {
                Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
                return null;
            }
            else
            {
                Requirement updatedRequirement = JsonUtility.FromJson<Requirement>(response.ResponseBody);
                return updatedRequirement;
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
            string url = baseUrl + "requirements/" + toUpdate.Id;

            // convert the requirement to a uploadable format (the statistic fields of the requirement are not recognized as input by the service)
            UploadableRequirement uploadableRequirement = toUpdate.ToUploadFormat();

            string json = JsonUtility.ToJson(uploadableRequirement);

            Response response = await Rest.PutAsync(url, json, null, -1, true);
            if (!response.Successful)
            {
                Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
                return null;
            }
            else
            {
                Requirement requirement = JsonUtility.FromJson<Requirement>(response.ResponseBody);
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