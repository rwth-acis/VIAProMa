using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.UI.ListView.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.DataModel.GitHub
{
    /// <summary>
    /// A generalized issue
    /// This data object contains data of both GitHubIssues and Requirements Bazaar requirements
    /// </summary>
    [Serializable]
    public class GitHubIssue : IListViewItemData
    {
        /// <summary>
        /// The data source where the issue is stored
        /// </summary>
        [SerializeField] private DataSource source;
        /// <summary>
        /// The id of the issue as it is stored on the data source
        /// </summary>
        [SerializeField] private int number;
        /// <summary>
        /// The name or title of the issue
        /// </summary>
        [SerializeField] private string title;
        /// <summary>
        /// A description of the issue
        /// </summary>
        [SerializeField] private string body;
        /// <summary>
        /// The id of the project which contains the issue
        /// </summary>
        [SerializeField] private int projectId;
        /// <summary>
        /// The user which created the issue
        /// </summary>
        [SerializeField] private GitHubUser user;
        /// <summary>
        /// The current status of the issue
        /// </summary>
        [SerializeField] private string state;
        /// <summary>
        /// Timestamp string when the issue was created
        /// </summary>
        [SerializeField] private string created_at;
        /// <summary>
        /// Timestamp string when the issue was last updated
        /// </summary>
        [SerializeField] private string updated_at;
        /// <summary>
        /// Timestamp string when the issue was closed
        /// </summary>
        [SerializeField] private string closed_at;
        /// <summary>
        /// The array containing all users who are currently developing the issue
        /// </summary>
        [SerializeField] private GitHubUser[] assignees;
        /// <summary>
        /// The array containing all users who have commented on the issue
        /// </summary>
        [SerializeField] private GitHubUser[] commenters;

        /// <summary>
        /// The data source of the issue
        /// </summary>
        public DataSource Source { get => source; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int Number { get => number; }
        /// <summary>
        /// The name/ title of the issue
        /// </summary>
        public string Title { get => title; }
        /// <summary>
        /// A longer description of the issue's content
        /// </summary>
        public string Body { get => body; }
        /// <summary>
        /// The id of the project to which this issue belongs
        /// </summary>
        public int ProjectId { get => projectId; }
        /// <summary>
        /// The user that created the issue
        /// </summary>
        public GitHubUser User { get => user; }
        /// <summary>
        /// The status of the issue (open, in progress or closed)
        /// </summary>
        public string State { get => state; }
        /// <summary>
        /// The string representation of the point in time when the issue was created
        /// </summary>
        public string CreationDateString { get => created_at; }
        /// <summary>
        /// The string representation of the point in time when the issue was last updated
        /// </summary>
        public string LastUpdatedDateString { get => updated_at; }
        /// <summary>
        /// The string representation of the point in time when the issue was closed
        /// </summary>
        public string ClosedDateString { get => closed_at; }
        /// <summary>
        /// The array of developers who are assigned to the issue and are working on it
        /// </summary>
        public GitHubUser[] Assignees { get => assignees; }
        /// <summary>
        /// The array of users who have commented on the issue
        /// </summary>
        public GitHubUser[] Commenters { get => commenters; }

        /// <summary>
        /// Creates an issue
        /// </summary>
        public GitHubIssue()
        {
        }

        /// <summary>
        /// Creats an issue with the given parameters
        /// </summary>
        /// <param name="source">The data source of the issue</param>
        /// <param name="number">The id of the issue</param>
        /// <param name="title">The name/ title of the issue</param>
        /// <param name="body">A longer description of the issue's content</param>
        /// <param name="projectId">The id of the project to which this issue belongs</param>
        /// <param name="user">The user that created the issue</param>
        /// <param name="state">The status of the issue (open, in progress or closed)</param>
        /// <param name="created_at">The string representation of the point in time when the issue was created</param>
        /// <param name="updated_at">The string representation of the point in time when the issue was last updated</param>
        /// <param name="closed_at">The string representation of the point in time when the issue was closed</param>
        /// <param name="assignees">The array of developers who are assigned to the issue and are working on it</param>
        /// <param name="commenters">The array of users who have commented on the issue</param>
        public GitHubIssue(DataSource source, int number, string title, string body, int projectId, GitHubUser user, string state, string created_at, string updated_at, string closed_at, GitHubUser[] assignees, GitHubUser[] commenters)
        {
            this.source = source;
            this.number = number;
            this.title = title;
            this.body = body;
            this.projectId = projectId;
            this.user = user;
            this.state = state;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.closed_at = closed_at;
            this.assignees = assignees;
            this.commenters = commenters;
        }

        /// <summary>
        /// Deep-comparion between this issue and obj based on the issue's source and id
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if obj is an issue and the source and ids match</returns>
        public override bool Equals(object obj)
        {
            GitHubIssue issue = obj as GitHubIssue;
            if (issue == null)
            {
                return false;
            }

            return (source == issue.source && number == issue.number);
        }

        /// <summary>
        /// Deep-comparion between this issue and the other issue obj based on the issue's source and id
        /// </summary>
        /// <param name="issue">The issue to compare to</param>
        /// <returns>True if the source and id of this issue and issue match</returns>
        public bool Equals(GitHubIssue issue)
        {
            if (issue == null)
            {
                return false;
            }
            return (source == issue.source && number == issue.number);
        }

        /// <summary>
        /// Gets a hash code of the issue object
        /// </summary>
        /// <returns>A has code</returns>
        public override int GetHashCode()
        {
            return (int)source ^ number;
        }
    }
}