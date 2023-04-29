using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    [Serializable]
    /// <summary>
    /// Represents a contributors of a requirement in the Requirements Bazaar
    /// </summary>
    public class Contributors : IListViewItemData, IUninitializable
    {
        /// <summary>
        /// The values of the contributors
        /// </summary>
        public ReqBazUser creator;
        public ReqBazUser leadDeveloper;
        public ReqBazUser[] developers;
        public ReqBazUser[] commentCreator;
        public ReqBazUser[] attachmentCreator;

        /// <summary>
        /// Whether or not the contributors have been initialized
        /// </summary>
        public bool IsUninitialized
        {
            get
            {
                if (creator.IsUninitialized && leadDeveloper.IsUninitialized && developers.Length == 0 && commentCreator.Length == 0 && attachmentCreator.Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}