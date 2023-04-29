using Org.Requirements_Bazaar.API;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.DataModel
{

    /// <summary>
    /// Represents a project in the Requirements Bazaar
    /// </summary>
    [Serializable]
    public class Project
    {

        /// <summary>
        /// The values of a project
        /// </summary>
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private bool visibility;
        [SerializeField] private int defaultCategoryId;
        [SerializeField] private User leader;
        [SerializeField] private string creationDate;
        [SerializeField] private string lastUpdatedDate;
        [SerializeField] private int numberOfCategories;
        [SerializeField] private int numberOfRequirements;
        [SerializeField] private int numberOfFollowers;
        [SerializeField] private bool isFollower;

        /// <summary>
        /// Creates a new project and assigns the currently logged in user as the leader
        /// </summary>
        /// <param name="name">The name/title of the project</param>
        /// <param name="description">The description of the project</param>
        /// <param name="visibility">True if the project should be public</param>
        public Project(string name, string description, bool visibility) : this(name, description, visibility, null)
        {
        }

        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="name">The name/title of the project</param>
        /// <param name="description">The description of the project</param>
        /// <param name="visibility">True if the project should be public</param>
        /// <param name="leader">The leader of the project</param>
        public Project(string name, string description, bool visibility, User leader)
        {
            this.name = name;
            this.description = description;
            this.visibility = visibility;
            this.leader = leader;
        }

        #region Properties

        /// <summary>
        /// The id of the project
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// The name/title of the project
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
        /// A description of the project content
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
        /// If true, the project is visible to other users
        /// </summary>
        public bool Visibility
        {
            get
            {
                return visibility;
            }

            set
            {
                visibility = value;
            }
        }

        /// <summary>
        /// The category id where requirements are stored by default if no category was specified for them
        /// </summary>
        public int DefaultCategoryId
        {
            get
            {
                return defaultCategoryId;
            }

            set
            {
                defaultCategoryId = value;
            }
        }

        /// <summary>
        /// The leader of the project
        /// </summary>
        public User Leader
        {
            get
            {
                return leader;
            }

            set
            {
                leader = value;
            }
        }

        /// <summary>
        /// Timestamp of the time when the project was created
        /// </summary>
        public string CreationDate
        {
            get
            {
                return creationDate;
            }
        }

        /// <summary>
        /// Timestamp of the time when the project was last updated
        /// </summary>
        public string LastUpdatedDate
        {
            get
            {
                return lastUpdatedDate;
            }
        }

        /// <summary>
        /// Counts the number of categories in this project
        /// </summary>
        public int NumberOfCategories
        {
            get
            {
                return numberOfCategories;
            }
        }

        /// <summary>
        /// Counts the number of requirements in this project
        /// </summary>
        public int NumberOfRequirements
        {
            get
            {
                return numberOfRequirements;
            }
        }

        /// <summary>
        /// Counts the number of users who are following this project
        /// </summary>
        public int NumberOfFollowers
        {
            get
            {
                return numberOfFollowers;
            }
        }

        /// <summary>
        /// True if the currently logged in user is following the project
        /// </summary>
        public bool IsFollower
        {
            get
            {
                return isFollower;
            }
        }

        #endregion
    }
}