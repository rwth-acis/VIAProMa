using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// Represents a category in the Requirements Bazaar
    /// </summary>
    [Serializable]
    public class Category : IListViewItemData
    {

        /// <summary>
        /// The values for a category
        /// </summary>
        public int id;
        public string name;
        public string description;
        public int projectId;
        public ReqBazUser leader;
        public string creationDate;
        public string lastUpdatedDate;
        public int numberOfRequirements;
        public int numberOfFollowers;

        /// <summary>
        /// Creates a blank category
        /// </summary>
        public Category()
        {
        }

        /// <summary>
        /// The Instantiator of a category
        /// </summary>
        /// <param name="id">The numeric id of the category</param>
        /// <param name="name">The name of the category</param>
        public Category(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}