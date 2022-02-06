using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using System;
using i5.VIAProMa.WebConnection;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Login;
using UnityEngine.Networking;
using System.Linq;

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
            if (ServiceManager.GetService<GitHubOidcService>() != null)
            {
                Debug.Log("Service not null");
            }
            headers.Add("Authorization", "token " + ServiceManager.GetService<GitHubOidcService>().AccessToken);
            headers.Add("Accept", "application/vnd.github.v3+json");
            string json = "{ \"title\": \"" + name + "\", \"body\": \"" + description + "\" }";

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
        /// <param name="issueID">The ID of the issue which should be edited</param>
        /// <param name="projectID">The ID of the project which the issue is part of</param>
        /// <param name="owner">The owner of the repository to which the issue belongs that should be edited</param>
        /// <param name="repositoryName">The name of the repository to which the issue belongs that should be edited</param>
        /// <param name="newName">The new name of the issue after editing</param>
        /// <param name="newDescription">The new description of the issue after editing</param>
        /// <returns>The edited issue</returns>
        public static async Task<Issue[]> EditIssue(int issueID, string owner, string repositoryName, string newName, string newDescription)
        {

            // Check for repository
            ApiResult<Issue[]> repositoryIssuesApiResult = await GitHub.GetIssuesInRepository(owner, repositoryName, 1, 100);
            Issue[] repositoryIssues = repositoryIssuesApiResult.Value;
            if (repositoryIssues == null || repositoryIssues.Length == 0)
            {
                Debug.LogError("RepositoryIssues not found");
            }

            Dictionary<string, string> headers = new Dictionary<string, string>();
            if (ServiceManager.GetService<GitHubOidcService>() != null)
            {
                Debug.Log("Service not null");
            }
            headers.Add("Authorization", "token " + ServiceManager.GetService<GitHubOidcService>().AccessToken);
            Debug.Log(ServiceManager.GetService<GitHubOidcService>().AccessToken);
            headers.Add("Accept", "application/vnd.github.v3+json");
            string json = "{ \"title\": \"" + newName + "\", \"body\": \"" + newDescription + "\" }";

            Response response = await PatchAsync(
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

        #region PATCH

        /// <summary>
        /// Rest PUT.
        /// </summary>
        /// <param name="query">Finalized Endpoint Query with parameters.</param>
        /// <param name="jsonData">Data to be submitted.</param>
        /// <param name="headers">Optional header information for the request.</param>
        /// <param name="timeout">Optional time in seconds before request expires.</param>
        /// <param name="readResponseData">Optional bool. If its true, response data will be read from web request download handler.</param>
        /// <returns>The response data.</returns>
        public static async Task<Response> PatchAsync(string query, string jsonData, Dictionary<string, string> headers = null, int timeout = -1, bool readResponseData = false)
        {
            using (var webRequest = UnityWebRequest.Put(query, jsonData))
            {
                webRequest.method = "PATCH";
                webRequest.SetRequestHeader("Content-Type", "application/json");
                return await ProcessRequestAsync(webRequest, timeout, headers, readResponseData);
            }
        }

        #endregion PATCH

        private static async Task<Response> ProcessRequestAsync(UnityWebRequest webRequest, int timeout, Dictionary<string, string> headers = null, bool readResponseData = false, CertificateHandler certificateHandler = null, bool disposeCertificateHandlerOnDispose = true)
        {
            if (timeout > 0)
            {
                webRequest.timeout = timeout;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            // HACK: Workaround for extra quotes around boundary.
            if (webRequest.method == UnityWebRequest.kHttpVerbPOST ||
                webRequest.method == UnityWebRequest.kHttpVerbPUT)
            {
                string contentType = webRequest.GetRequestHeader("Content-Type");
                if (contentType != null)
                {
                    contentType = contentType.Replace("\"", "");
                    webRequest.SetRequestHeader("Content-Type", contentType);
                }
            }

            webRequest.certificateHandler = certificateHandler;
            webRequest.disposeCertificateHandlerOnDispose = disposeCertificateHandlerOnDispose;
            await webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
#else
            if (webRequest.isNetworkError || webRequest.isHttpError)
#endif // UNITY_2020_1_OR_NEWER
            {
                if (webRequest.responseCode == 401) { return new Response(false, "Invalid Credentials", null, webRequest.responseCode); }

                if (webRequest.GetResponseHeaders() == null)
                {
                    return new Response(false, "Device Unavailable", null, webRequest.responseCode);
                }

                string responseHeaders = webRequest.GetResponseHeaders().Aggregate(string.Empty, (current, header) => $"\n{header.Key}: {header.Value}");
                string downloadHandlerText = webRequest.downloadHandler?.text;
                Debug.LogError($"REST Error: {webRequest.responseCode}\n{downloadHandlerText}{responseHeaders}");
                return new Response(false, $"{responseHeaders}\n{downloadHandlerText}", webRequest.downloadHandler?.data, webRequest.responseCode);
            }
            if (readResponseData)
            {
                return new Response(true, webRequest.downloadHandler?.text, webRequest.downloadHandler?.data, webRequest.responseCode);
            }
            else // This option can be used only if action will be triggered in the same scope as the webrequest
            {
                return new Response(true, new Task<string>(() => webRequest.downloadHandler?.text), () => webRequest.downloadHandler?.data, webRequest.responseCode);
            }
        }
    }
}