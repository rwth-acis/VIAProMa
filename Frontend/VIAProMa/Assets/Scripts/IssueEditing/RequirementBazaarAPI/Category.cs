using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.DataModel
{

    [Serializable]
    public class Category
    {
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

        public Category(string name, string description, int projectId)
        {
            this.name = name;
            this.description = description;
            this.projectId = projectId;
        }
    }

}