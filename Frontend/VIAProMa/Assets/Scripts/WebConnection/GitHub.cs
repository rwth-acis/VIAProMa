using i5.VIAProMa.DataModel;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.WebConnection
{
    /// <summary>
    /// Contains calls to the backend concerning GitHub content
    /// The class posts requests to the ViaProMa backend which then accesses GitHub
    /// </summary>
    public static class GitHub
    {
        /// <summary>
        /// Gets the issues of a GitHub repository on the given page
        /// </summary>
        /// <param name="owner">The owner of the repository</param>
        /// <param name="repositoryName">The name of the repository</param>
        /// <param name="page">The page of the content</param>
        /// <param name="itemsPerPage">States how many issues should be displayed on one page</param>
        /// <returns>An array of issues in the repository; contained in an APIResult object</returns>
        public static async Task<ApiResult<Issue[]>> GetIssuesInRepository(string owner, string repositoryName, int page, int itemsPerPage)
        {
            Response resp = await Rest.GetAsync(
                ConnectionManager.Instance.BackendAPIBaseURL + "gitHub/repos/" + owner + "/" + repositoryName + "/issues?page=" + page + "&per_page=" + itemsPerPage,
                null,
                -1,
                null,
                true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return new ApiResult<Issue[]>(resp.ResponseCode, resp.ResponseBody);
            }
            else
            {
                Issue[] issues = JsonArrayUtility.FromJson<Issue>(resp.ResponseBody);
                foreach (Issue issue in issues)
                {
                    IssueCache.AddIssue(issue);
                }
                return new ApiResult<Issue[]>(issues);
            }
        }

        /// <summary>
        /// Gets a specific issue by its number in the given repository
        /// </summary>
        /// <param name="repositoryId">The numeric id of the repository</param>
        /// <param name="issueNumber">The (local) issue number in the repository</param>
        /// <returns>The issue; contained in an APIResult object</returns>
        public static async Task<ApiResult<Issue>> GetIssue(int repositoryId, int issueNumber)
        {
            Issue cached = IssueCache.GetGitHubIssue(repositoryId, issueNumber);
            if (cached != null)
            {
                return new ApiResult<Issue>(cached);
            }

            Response resp = await Rest.GetAsync(
                ConnectionManager.Instance.BackendAPIBaseURL + "gitHub/repositories/" + repositoryId + "/issues/" + issueNumber,
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

        public static async Task<ApiResult<PunchCardEntry[]>> GetGitHubPunchCard(string owner, string repository)
        {
            Response resp = await Rest.GetAsync(
                ConnectionManager.Instance.BackendAPIBaseURL + "githubPunchCard/" + owner + "/" + repository,
                null,
                -1,
                null,
                true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            if (!resp.Successful)
            {
                Debug.LogError(resp.ResponseCode + ": " + resp.ResponseBody);
                return new ApiResult<PunchCardEntry[]>(resp.ResponseCode, resp.ResponseBody);
            }
            else
            {
                PunchCardEntry[] gitHubPunchCard = JsonArrayUtility.FromJson<PunchCardEntry>(resp.ResponseBody);
                return new ApiResult<PunchCardEntry[]>(gitHubPunchCard);
            }
        }
    }
}