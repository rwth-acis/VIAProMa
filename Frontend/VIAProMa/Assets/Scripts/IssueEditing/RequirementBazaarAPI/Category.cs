using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.DataModel
{

    /// <summary>
    /// Represents a category in the Requirements Bazaar
    /// </summary>
    [Serializable]
    public class Category
    {

        /// <summary>
        /// The values for a category
        /// </summary>
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int projectId;
        [SerializeField] private User leader;
        [SerializeField] private string creationDate;
        [SerializeField] private string lastUpdatedDate;
        [SerializeField] private int numberOfRequirements;
        [SerializeField] private int numberOfFollowers;
        [SerializeField] private bool isFollower;

        #region Properties

        public int Id
        {
            get
            {
                return id;
            }
        }

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

        public string CreationDate
        {
            get
            {
                return creationDate;
            }
        }

        public string LastUpdatedDate
        {
            get
            {
                return lastUpdatedDate;
            }
        }

        public int NumberOfRequirements
        {
            get
            {
                return numberOfRequirements;
            }
        }

        public int NumberOfFollowers
        {
            get
            {
                return numberOfFollowers;
            }
        }

        public bool IsFollower
        {
            get
            {
                return isFollower;
            }
        }

        #endregion


        /// <summary>
        /// The Instantiator of a category
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <param name="description">the description for the category</param>
        /// <param name="projectId">the id for the category</param>
        public Category(string name, string description, int projectId)
        {
            this.name = name;
            this.description = description;
            this.projectId = projectId;
        }
    }

}