using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// A generalized issue
    /// This data object contains data of both GitHubIssues and Requirements Bazaar requirements
    /// </summary>
    [Serializable]
    public class RequirementIssue : IListViewItemData
    {
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
        [SerializeField] private ReqBazUser creator;
        /// <summary>
        /// The categories
        /// </summary>
        [SerializeField] private int[] categories;
        /// <summary>
        /// The tags
        /// </summary>
        [SerializeField] private string[] tags;
        /// <summary>
        /// The current status of the issue
        /// </summary>
        [SerializeField] private IssueStatus status;
        /// <summary>
        /// Timestamp string when the issue was created
        /// </summary>
        [SerializeField] private string creationDate;
        /// <summary>
        /// Timestamp string when the issue was last updated
        /// </summary>
        [SerializeField] private string lastUpdatedDate;
        /// <summary>
        /// The user with the last activity on the issue
        /// </summary>
        [SerializeField] private ReqBazUser lastActivityUser;
        /// <summary>
        /// The last activity
        /// </summary>
        [SerializeField] private string lastActivity;
        /// <summary>
        /// The last activity
        /// </summary>
        [SerializeField] private int numberOfComments;
        /// <summary>
        /// The last activity
        /// </summary>
        [SerializeField] private int numberOfAttachments;
        /// <summary>
        /// The last activity
        /// </summary>
        [SerializeField] private int numberOfFollowers;
        /// <summary>
        /// The last activity
        /// </summary>
        [SerializeField] private int upVotes;
        /// <summary>
        /// The last activity
        /// </summary>
        [SerializeField] private int downVotes;

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
        public ReqBazUser Creator { get => creator; }
        /// <summary>
        /// The categories
        /// </summary>
        public int[] Categories { get => categories; }
        /// <summary>
        /// The tags
        /// </summary>
        public string[] Tags { get => tags; }
        /// <summary>
        /// The status of the issue (open, in progress or closed)
        /// </summary>
        public IssueStatus Status { get => status; }
        /// <summary>
        /// The string representation of the point in time when the issue was created
        /// </summary>
        public string CreationDateString { get => creationDate; }
        /// <summary>
        /// The string representation of the point in time when the issue was created
        /// </summary>
        public string LastUpdatedDateString { get => lastUpdatedDate; }
        /// <summary>
        /// The user that created the issue
        /// </summary>
        public ReqBazUser LastActivityUser { get => lastActivityUser; }
        /// <summary>
        /// A longer description of the issue's content
        /// </summary>
        public string LastActivity { get => lastActivity; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int NumberOfComments { get => numberOfComments; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int NumberOfAttachments { get => numberOfAttachments; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int NumberOfFollowers { get => numberOfFollowers; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int UpVotes { get => upVotes; }
        /// <summary>
        /// The id of the issue
        /// </summary>
        public int DownVotes { get => downVotes; }

        /// <summary>
        /// Creates an issue
        /// </summary>
        public RequirementIssue()
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
        /// <param name="categories">The categories</param>
        /// <param name="status">The status of the issue (open, in progress or closed)</param>
        /// <param name="creationDate">The string representation of the point in time when the issue was created</param>
        /// <param name="developers">The array of developers who are assigned to the issue and are working on it</param>
        public RequirementIssue(int id, string name, string description, int projectId, ReqBazUser creator, int[] categories, string[] tags, IssueStatus status, string creationDate, string lastUpdatedDate, ReqBazUser lastActivityUser, string lastActivity, int numberOfComments, int numberOfAttachments, int numberOfFollowers, int upVotes, int downVotes)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.projectId = projectId;
            this.creator = creator;
            this.categories = categories;
            this.status = status;
            this.creationDate = creationDate;
            this.tags = tags;
            this.lastUpdatedDate = lastUpdatedDate;
            this.lastActivityUser = lastActivityUser;
            this.lastActivity = lastActivity;
            this.numberOfComments = numberOfComments;
            this.numberOfAttachments = numberOfAttachments;
            this.numberOfFollowers = numberOfFollowers;
            this.upVotes = upVotes;
            this.downVotes = downVotes;
        }



        /// <summary>
        /// Deep-comparion between this issue and obj based on the issue's source and id
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if obj is an issue and the source and ids match</returns>
        public override bool Equals(object obj)
        {
            RequirementIssue issue = obj as RequirementIssue;
            if (issue == null)
            {
                return false;
            }

            return (id == issue.id);
        }

        /// <summary>
        /// Deep-comparion between this issue and the other issue obj based on the issue's source and id
        /// </summary>
        /// <param name="issue">The issue to compare to</param>
        /// <returns>True if the source and id of this issue and issue match</returns>
        public bool Equals(RequirementIssue issue)
        {
            if (issue == null)
            {
                return false;
            }
            return (id == issue.id);
        }

        /// <summary>
        /// Gets a hash code of the issue object
        /// </summary>
        /// <returns>A has code</returns>
        public override int GetHashCode()
        {
            return (int)0 ^ id;
        }
    }
}