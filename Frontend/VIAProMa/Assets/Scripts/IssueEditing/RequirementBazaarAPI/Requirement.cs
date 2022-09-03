using Org.Requirements_Bazaar.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.DataModel
{
    /// <summary>
    /// A requirement
    /// </summary>
    [Serializable]
    public class Requirement
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int projectId;
        [SerializeField] private User creator;
        [SerializeField] private int[] categories;
        [SerializeField] private string creationDate;
        [SerializeField] private int numberOfComments;
        [SerializeField] private int numberOfAttachments;
        [SerializeField] private int numberofFollowers;
        [SerializeField] private int upVotes;
        [SerializeField] private int downVotes;
        [SerializeField] private string userVoted;

        #region Properties

        /// <summary>
        /// The id of the requirement
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// The name/ title of the requirement
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// The description of the requirement's content
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        /// <summary>
        /// The id of the project to which the requirement belongs
        /// </summary>
        public int ProjectId
        {
            get
            {
                return projectId;
            }

            set
            {
                projectId = value;
            }
        }

        /// <summary>
        /// The user who created the requirement
        /// </summary>
        public User Creator
        {
            get
            {
                return creator;
            }
        }

        /// <summary>
        /// An array of categories to which the requirement belongs
        /// </summary>
        public int[] Categories
        {
            get
            {
                return categories;
            }

            set
            {
                categories = value;
            }
        }

        /// <summary>
        /// Timestamp of the time when the requirement was created
        /// </summary>
        public string CreationDate
        {
            get
            {
                return creationDate;
            }
        }

        /// <summary>
        /// Counts the number of comments that this requirement has
        /// </summary>
        public int NumberOfComments
        {
            get
            {
                return numberOfComments;
            }
        }

        /// <summary>
        /// Counts the number of attachments that this requirement has
        /// </summary>
        public int NumberOfAttachments
        {
            get
            {
                return numberOfAttachments;
            }
        }

        /// <summary>
        /// Counts the number of followers that this requirement has
        /// </summary>
        public int NumberofFollowers
        {
            get
            {
                return numberofFollowers;
            }
        }

        /// <summary>
        /// Counts the number of upvotes that this requirement was given by users
        /// </summary>
        public int UpVotes
        {
            get
            {
                return upVotes;
            }
        }

        /// <summary>
        /// Counts the number of downvotes that this requirement was given by users
        /// </summary>
        public int DownVotes
        {
            get
            {
                return downVotes;
            }
        }

        /// <summary>
        /// True if the currently logged in user has voted for this requirement
        /// </summary>
        public UserVoted UserVoted
        {
            get
            {
                if (userVoted == "UP_VOTE")
                {
                    return UserVoted.UP_VOTE;
                }
                else if (userVoted == "DOWN_VOTE")
                {
                    return UserVoted.DOWN_VOTE;
                }
                else if (userVoted == "NO_VOTE")
                {
                    return UserVoted.NO_VOTE;
                }
                else
                {
                    Debug.LogWarning("unrecognized user voted format");
                    return UserVoted.NO_VOTE;
                }
            }
        }

        #endregion

        /// <summary>
        /// Converts the requirement to a UploadableRequirement
        /// The UploadableRequirement has a format which is accepted by the ReqBaz server when uploading a requirement
        /// </summary>
        /// <returns>The UploadableRequirement which can be serialized and uploaded</returns>
        public UploadableRequirement ToUploadFormat()
        {
            UploadableRequirement req = new UploadableRequirement(id, name, description, projectId, categories);
            return req;
        }
    }

    public enum UserVoted
    {
        NO_VOTE, UP_VOTE, DOWN_VOTE
    }

}