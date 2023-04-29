using System;

namespace i5.VIAProMa.DataModel.ReqBaz
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
        public int id;
        public string name;
        public string description;
        public bool visibility;
        public int defaultCategoryId;
        public ReqBazUser leader;
        public string creationDate;
        public string lastUpdatedDate;
        public int numberOfCategories;
        public int numberOfRequirements;
        public int numberOfFollowers;
        public bool isFollower;
    }
}