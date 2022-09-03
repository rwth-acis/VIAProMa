using Org.Requirements_Bazaar.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Org.Requirements_Bazaar.Serialization
{

    /// <summary>
    /// Used to serialize post data because the API does not accept a fully-serialized requirement
    /// </summary>
    [Serializable]
    public class UploadableRequirement
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private int projectId;
        [SerializeField] private int[] categories;

        public UploadableRequirement(string name, string description, int projectId, int[] categories)
            : this(0, name, description, projectId, categories)
        {
        }

        public UploadableRequirement(int id, string name, string description, int projectId, int[] categories)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.projectId = projectId;
            this.categories = categories;
        }

        #region Properties

        /// <summary>
        /// The id of the requirement
        /// This id is 0 if the uploadable requirement was created using a constructor
        /// It is only filled with a value if the uploadable requirement was converted from an existing requirement
        /// </summary>
        public int Id
        {
            get
            {
                return id;
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
        /// An array of categories to which the requirement belongs
        /// If it is null or has a length of 0, the requirement will be assigned to the default category of the project
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

        #endregion
    }

}