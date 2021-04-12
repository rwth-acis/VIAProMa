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
using i5.VIAProMa.DataModel.API;
using Org.Requirements_Bazaar.DataModel;

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
        public static async Task<Issue> CreateIssue(string owner, string repositoryName, string name, string description)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub) != null)
            {
                Debug.Log("Service not null");
            }
            headers.Add("Authorization", "Bearer " + ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub).AccessToken);
            Debug.Log(ConnectionManager.Instance.BackendAPIBaseURL + "gitHub/repos/" + owner + "/" + repositoryName + "/issues?title=" + name + "&body=" + description);
            Response resp = await Rest.PostAsync(
                "https://api.github.com/" + "repos/" + owner + "/" + repositoryName + "/issues?title=" + name + "&body=" + description,
                headers,
                -1,
                true);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
            }
            Issue issue = JsonUtility.FromJson<Issue>(resp.ResponseBody);
            return issue;
        }

        /// <summary>
        /// Edits a specific issue by its id
        /// </summary>
        /// <param name="issueName">The name of the issue which should be edited</param>
        /// <param name="projectID">The ID of the project which the issue is part of</param>
        /// <param name="owner">The owner of the repository to which the issue belongs that should be edited</param>
        /// <param name="repositoryName">The name of the repository to which the issue belongs that should be edited</param>
        /// <param name="newName">The new name of the issue after editing</param>
        /// <param name="newDescription">The new description of the issue after editing</param>
        /// <returns>The edited issue</returns>
        public static async Task<Issue> EditIssue(int id, string owner, string repositoryName, string newName, string newDescription)
        {
            //Wie bekomme ich den Issue Namen?
        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    if (ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub) != null)
        //    {
        //        Debug.Log("Service not null");
        //    }
        //    headers.Add("Authorization", "Bearer " + ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub).AccessToken);

        //    Response response = await Rest.PatchAsync(
        //        "https://api.github.com/" + "repos/" + owner + "/" + repositoryName + "/issues/"+id+"?title=" + name + "&body=" + description,
        //         headers,
        //        -1,
        //       true);
        //    if (!response.Successful)
        //    {
        //        Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
        //        return null;
        //    }
        //    else
        //    {
        //        Issue issue = JsonUtility.FromJson<Issue>(resp.ResponseBody);
        //        return issue;
        //    }
        }
    }
}