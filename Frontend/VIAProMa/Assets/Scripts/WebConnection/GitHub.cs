using i5.VIAProMa.DataModel;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.GitHub;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;
using i5.Toolkit.Core.Utilities;

namespace i5.VIAProMa.WebConnection
{
    /// <summary>
    /// Contains calls to the backend concerning GitHub content
    /// The class posts requests to the ViaProMa backend which then accesses GitHub
    /// </summary>
    public static class GitHub
    {
        public static IRestConnector RestConnector = new UnityWebRequestRestConnector();
        public static IJsonSerializer JsonSerializer = new JsonUtilityAdapter();

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
            WebResponse<string> resp = await RestConnector.GetAsync("https://api.github.com/" + "repos/" + owner + "/" + repositoryName + "/issues?page=" + page + "&per_page=" + itemsPerPage, null);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            string responseBody = resp.Content;
            responseBody = "{\"array\":" + responseBody + "}";
            if (!resp.Successful)
            {
                Debug.LogError(resp.Code + ": " + responseBody);
                return new ApiResult<Issue[]>(resp.Code, responseBody);
            }
            else
            {
                GitHubIssue[] githubissues = Utilities.JsonArrayUtility.FromJson<GitHubIssue>(responseBody);
                Issue[] issues = Issue.fromGitHubIssues(githubissues);
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

            WebResponse<string> resp = await RestConnector.GetAsync("https://api.github.com/" + "repositories/" + repositoryId + "/issues/" + issueNumber, null);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            string responseBody = resp.Content;
            if (!resp.Successful)
            {
                Debug.LogError(resp.Code + ": " + responseBody);
                return new ApiResult<Issue>(resp.Code, resp.Content);
            }
            else
            {
                GitHubIssue githubissue = JsonUtility.FromJson<GitHubIssue>(responseBody);
                Issue issue = Issue.fromGitHubIssue(githubissue);
                IssueCache.AddIssue(issue);
                return new ApiResult<Issue>(issue);
            }
        }

        public static async Task<ApiResult<PunchCardEntry[]>> GetGitHubPunchCard(string owner, string repository)
        {
            WebResponse<string> resp = await RestConnector.GetAsync("https://api.github.com/" + "githubPunchCard/" + owner + "/" + repository, null);
            ConnectionManager.Instance.CheckStatusCode(resp.Code);
            string responseBody = resp.Content;
            if (!resp.Successful)
            {
                Debug.LogError(resp.Code + ": " + responseBody);
                return new ApiResult<PunchCardEntry[]>(resp.Code, responseBody);
            }
            else
            {
                PunchCardEntry[] gitHubPunchCard = Utilities.JsonArrayUtility.FromJson<PunchCardEntry>(responseBody);
                return new ApiResult<PunchCardEntry[]>(gitHubPunchCard);
            }
        }
    }
}