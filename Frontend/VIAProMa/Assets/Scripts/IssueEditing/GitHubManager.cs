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
using i5.VIAProMa.Utilities;
using System.Net.Http;

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
            headers.Add("Authorization", "token " + ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub).AccessToken);
            headers.Add("Accept", "application/vnd.github.v3+json");
            string json = "{ \"title\": \"" + name + "\", \"body\": \"" + description + "\" }";

            Debug.Log(ConnectionManager.Instance.BackendAPIBaseURL + "gitHub/repos/" + owner + "/" + repositoryName + "/issues?title=" + name + "&body=" + description);
            Response resp = await Rest.PostAsync(
                "https://api.github.com/" + "repos/" + owner + "/" + repositoryName + "/issues",
                json,
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
        public static async Task<Issue[]> EditIssue(string issueName, string owner, string repositoryName, string newName, string newDescription)
        {
            //Retrieving the IssueID
            ApiResult<Issue[]> repositoryIssuesApiResult = await GitHub.GetIssuesInRepository(owner, repositoryName, 1, 100);
            Issue[] repositoryIssues = repositoryIssuesApiResult.Value;
            if (repositoryIssues == null || repositoryIssues.Length == 0)
            {
                Debug.LogError("RepositoryIssues not found");
            }
            int issueID = 0;
            for (int i = 0; i < repositoryIssues.Length; i++)
            {
                if (repositoryIssues[i].Name == issueName)
                {
                    issueID = repositoryIssues[i].Id;
                    break;
                }
            
            }
            if (issueID == 0)
            {
               Debug.LogError("Issue not found");
                return null;
            }
            Debug.Log(issueID);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub) != null)
            {
                Debug.Log("Service not null");
            }
            headers.Add("Authorization", "token " + ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.GitHub).AccessToken);
            headers.Add("Accept", "application/vnd.github.v3+json");
            string json = "{ \"title\": \"" + newName + "\", \"body\": \"" + newDescription + "\" }";

            Response response = await Rest.PatchAsync(
                     "https://api.github.com/" + "repos/" + owner + "/" + repositoryName + "/issues/" + issueID,
                     json,
                     headers,
                     -1,
                     true);
            if (!response.Successful)
            {
                 Debug.LogError(response.ResponseCode + ": " + response.ResponseBody);
                 return null ;
            }
            else
            {
                //Issue[] repositoryIssues = await GetIssuesFromRepository(owner, repositoryName);
                return repositoryIssues;
            }
        }

        /// Gets the issues of a GitHub repository on the given page
        /// </summary>
        /// <param name="owner">The owner of the repository</param>
        /// <param name="repositoryName">The name of the repository</param>
        /// <returns>An array of issues in the repository</returns>
        public static async Task<Issue[]> GetIssuesFromRepository(string owner, string repositoryName)
        {
            Response resp = await Rest.GetAsync(
                "https://api.github.com/" + "repos/" + owner + "/" + repositoryName + "/issues",
                null,
                -1,
                null,
                true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return null;
            }
            else
            {
                String json = "{ \"data\": " + resp.ResponseBody + " }";
                Debug.Log(json);
                Issue[] issues = JsonArrayUtility.FromJson<Issue>(json);
                if (issues == null || issues.Length == 0)
                {
                    Debug.Log("Array empty!");
                }
                foreach (Issue issue in issues)
                {
                    IssueCache.AddIssue(issue);
                }
                return issues;
            }
        }
    }
}