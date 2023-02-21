﻿using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// Represents a category in the Requirements Bazaar
    /// </summary>
    [Serializable]
    public class Category : IListViewItemData
    {
        public int id;
        public string name;
        public string description;
        public int projectId;
        public ReqBazUser creator;
        public string creationDate;
        public string lastActivity;
        public int numberOfRequirements;
        public int numberOfFollowers;
        //public Context userContext;

        public Category()
        {
        }

        public Category(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}