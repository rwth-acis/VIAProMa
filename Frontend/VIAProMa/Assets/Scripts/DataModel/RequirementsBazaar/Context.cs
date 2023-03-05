using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// Represents a category in the Requirements Bazaar
    /// </summary>
    [Serializable]
    public class Context : IListViewItemData
    {
        public string userVoted;
        public bool isMoveAllowed;
        public bool isDeleteAllowed;

        public Context()
        {
        }
    }
}