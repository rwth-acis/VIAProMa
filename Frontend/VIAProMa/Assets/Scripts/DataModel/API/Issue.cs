using i5.VIAProMa.UI.ListView.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.DataModel.GitHub;
using i5.VIAProMa.WebConnection;

namespace i5.VIAProMa.DataModel.API
{
    /// <summary>
    /// A generalized issue
    /// This data object contains data of both GitHubIssues and Requirements Bazaar requirements
    /// </summary>
    [Serializable]
    public class Issue : IListViewItemData
    {
        /// <summary>
        /// The data source where the issue is stored
        /// </summary>
        [SerializeField] private DataSource source;
        /// <summary>
        /// The id of the issue as it is stored on the data source
        /// </summary>
        [SerializeField] private int id;
        /// <summary>
        /// The name or title of the issue
        /// </summary>
        [SerializeField] private string name;
        /// <summary>
        /// A description of the issue
        /// </summary>
        [SerializeField] private string description;
        /// <summary>
        /// The id of the project which contains the issue
        /// </summary>
        [SerializeField] private int projectId;
        /// <summary>
        /// The user which created the issue
        /// </summary>
        [SerializeField] private User creator;
        /// <summary>
        /// The current status of the issue
        /// </summary>
        [SerializeField] private IssueStatus status;
        /// <summary>
        /// Timestamp string when the issue was created
        /// </summary>
        [SerializeField] private string creationDate;
        /// <summary>
        /// Timestamp string when the issue was closed
        /// </summary>
        [SerializeField] private string closedDate;
        /// <summary>
        /// The array containing all users who are currently developing the issue
        /// </summary>
        [SerializeField] private User[] developers;
        /// <summary>
        /// The array containing all users who have commented on the issue
        /// </summary>
        [SerializeField] private User[] commenters;
        /// <summary>
        /// The array containing all contributors
        /// </summary>
        [SerializeField] private Contributors contributors;

        /// <summary>
        /// The data source of the issue
        /// </summary>
        public DataSource Source { get => source; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int Id { get => id; }
        /// <summary>
        /// The name/ title of the issue
        /// </summary>
        public string Name { get => name; }
        /// <summary>
        /// A longer description of the issue's content
        /// </summary>
        public string Description { get => description; }
        /// <summary>
        /// The id of the project to which this issue belongs
        /// </summary>
        public int ProjectId { get => projectId; }
        /// <summary>
        /// The user that created the issue
        /// </summary>
        public User Creator { get => creator; }
        /// <summary>
        /// The status of the issue (open, in progress or closed)
        /// </summary>
        public IssueStatus Status { get => status; }
        /// <summary>
        /// The string representation of the point in time when the issue was created
        /// </summary>
        public string CreationDateString { get => creationDate; }
        /// <summary>
        /// The string representation of the point in time when the issue was closed
        /// </summary>
        public string ClosedDateString { get => closedDate; }
        /// <summary>
        /// The array of developers who are assigned to the issue and are working on it
        /// </summary>
        public User[] Developers { get => developers; }
        /// <summary>
        /// The array of users who have commented on the issue
        /// </summary>
        public User[] Commenters { get => commenters; }
        /// <summary>
        /// The array of users who have commented on the issue
        /// </summary>
        public Contributors Contributors { get => contributors; set => contributors = value; }

        /// <summary>
        /// Creates an issue
        /// </summary>
        public Issue()
        {
        }

        /// <summary>
        /// Creats an issue with the given parameters
        /// </summary>
        /// <param name="source">The data source of the issue</param>
        /// <param name="id">The id of the issue</param>
        /// <param name="name">The name/ title of the issue</param>
        /// <param name="description">A longer description of the issue's content</param>
        /// <param name="projectId">The id of the project to which this issue belongs</param>
        /// <param name="creator">The user that created the issue</param>
        /// <param name="status">The status of the issue (open, in progress or closed)</param>
        /// <param name="creationDate">The string representation of the point in time when the issue was created</param>
        /// <param name="closedDate">The string representation of the point in time when the issue was closed</param>
        /// <param name="developers">The array of developers who are assigned to the issue and are working on it</param>
        /// <param name="commenters">The array of users who have commented on the issue</param>
        public Issue(DataSource source, int id, string name, string description, int projectId, User creator, IssueStatus status, string creationDate, string closedDate, User[] developers, User[] commenters)
        {
            this.source = source;
            this.id = id;
            this.name = name;
            this.description = description;
            this.projectId = projectId;
            this.creator = creator;
            this.status = status;
            this.creationDate = creationDate;
            this.closedDate = closedDate;
            this.developers = developers;
            this.commenters = commenters;
        }

        /**
     * Generates a CrossIssue object from a Requirement (from the Requirements Bazaar)
     * @param req The requirement from the requirements bazaar
     * @return corresponding CrossIssue
     */
        public static Issue fromRequirement(Requirement req)
        {
            // we need the contributors in order to determine the developers and the issue status;
            if (req.Contributors != null)
            {
                Contributors contributors = null;//req.contributors;
                User[] developers;
                // construct the list of developers
                developers = User.fromReqBazUsers(contributors.developers);

                Issue issue = new Issue(DataSource.REQUIREMENTS_BAZAAR,
                        req.getId(),
                        req.getName(),
                        req.getDescription(),
                        req.getProjectId(),
                        User.fromReqBazUser(req.getCreator()),
                        determineIssueStatusFromRequirement(req, contributors),
                        req.getCreationDate(),
                        req.getLastUpdatedDate(),
                        //req.getRealized(),
                        developers,
                        User.fromReqBazUsers(contributors.commentCreator)
                        );
                return issue;
            }
            else
            {
                return null;
            }
        }

        public static Issue[] fromRequirements(Requirement[] reqs)
        {
            Issue[] issues = new Issue[reqs.Length];
            for (int i = 0; i < reqs.Length; i++)
            {
                issues[i] = fromRequirement(reqs[i]);
            }
            return issues;
        }

        /**
     * Determines the issue status based on a requirement and its contributors
     * @param req The requirement from the Requirements Bazaar
     * @param contributors The contributors of the requirement
     * @return The status of the corresponding issue
     */
        private static IssueStatus determineIssueStatusFromRequirement(Requirement req, Contributors contributors)
        {
            if (req.getRealized() != null)
            {
                return IssueStatus.CLOSED;
            }
            else
            {
                if (contributors.developers.Length > 0)
                {
                    return IssueStatus.IN_PROGRESS;
                }
                else
                {
                    return IssueStatus.OPEN;
                }
            }
        }

        public static Issue fromGitHubIssue(GitHubIssue gitHubIssue, int repositoryId)
        {
            Issue issue = new Issue(
                    DataSource.GITHUB,
                    gitHubIssue.getNumber(),
                    gitHubIssue.getTitle(),
                    gitHubIssue.getBody(),
                    repositoryId,
                    User.fromGitHubUser(gitHubIssue.getUser()),
                    gitHubIssue.getIssueStatus(),
                    gitHubIssue.getCreated_at(),
                    //gitHubIssue.getUpdated_at(),
                    gitHubIssue.getClosed_at(),
                    User.fromGitHubUsers(gitHubIssue.getAssignees()),
                    null);
            return issue;
        }

        public static Issue[] fromGitHubIssues(GitHubIssue[] gitHubIssues, int repositoryId)
        {
            Issue[] issues = new Issue[gitHubIssues.Length];
            for (int i = 0; i < gitHubIssues.Length; i++)
            {
                issues[i] = Issue.fromGitHubIssue(gitHubIssues[i], repositoryId);
            }
            return issues;
        }

        /// <summary>
        /// Deep-comparion between this issue and obj based on the issue's source and id
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if obj is an issue and the source and ids match</returns>
        public override bool Equals(object obj)
        {
            Issue issue = obj as Issue;
            if (issue == null)
            {
                return false;
            }

            return (source == issue.source && id == issue.id);
        }

        /// <summary>
        /// Deep-comparion between this issue and the other issue obj based on the issue's source and id
        /// </summary>
        /// <param name="issue">The issue to compare to</param>
        /// <returns>True if the source and id of this issue and issue match</returns>
        public bool Equals(Issue issue)
        {
            if (issue == null)
            {
                return false;
            }
            return (source == issue.source && id == issue.id);
        }

        /// <summary>
        /// Gets a hash code of the issue object
        /// </summary>
        /// <returns>A has code</returns>
        public override int GetHashCode()
        {
            return (int)source ^ id;
        }
    }
}