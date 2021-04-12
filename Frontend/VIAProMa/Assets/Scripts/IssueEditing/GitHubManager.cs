using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using System;
using System.Collections;
using UnityEngine.Networking;
using i5.VIAProMa.WebConnection;

namespace Org.Git_Hub.API
{

    public static class GitHubManager
    {

        /// <summary>
        /// Creates and posts a new issue
        /// </summary>
        /// <param name="owner">The owner of the repository where the issue will be posted</param>
        /// /// <param name="repository">The repository where the issue will be posted</param>
        /// <param name="name">The name/title of the issue</param>
        /// <param name="description">The description of the issue</param>
        /// <returns>The resulting issue as it was saved on the server</returns>
        public static async Task<GameObject> CreateIssue(string owner, string repositoryName, string name, string description)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub) != null)
            {
                Debug.Log("Service not null");
            }
            headers.Add("Authorization", "Bearer " + ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub).AccessToken);
            Response resp = await Rest.GetAsync(
                ConnectionManager.Instance.BackendAPIBaseURL + "gitHub/repos/" + owner + "/" + repositoryName + "/issues?title=" + name + "&body=" + description,
                headers,
                -1,
                null,
                true);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            }
            return null;
        }

        /// <summary>
        /// Edits a specific requirement by its id
        /// </summary>
        /// <param name="requirementName">The name of the requirement which should be deleted</param>
        /// /// <param name="projectId">The id of the project of the requirement which should be deleted</param>
        /// <returns>The deleted requirement</returns>
        //public static async Task<GameObject> EditIssue(string requirementName, string owner, string repositoryName, string newName, string newDescription)
        //{
        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    if (ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers) != null)
        //    {
        //        Debug.Log("Service not null");
        //    }
        //    headers.Add("Authorization", "Bearer " + ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).AccessToken);

        //    //Response response = await Rest.PutAsync("", null, headers, -1, true);
        //    if (!response.Successful)
        //    {
        //        Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
        //        return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}