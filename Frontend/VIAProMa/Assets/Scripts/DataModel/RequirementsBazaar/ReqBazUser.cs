using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// Represents a user in the Requirements Bazaar
    /// </summary>
    [Serializable]
    public class ReqBazUser : IListViewItemData, IUninitializable
    {

        /// <summary>
        /// The values of the requirements bazaar user
        /// </summary>
        public int id;
        public string userName;
        public string firstName;
        public string lastName;
        public bool admin;
        public long las2peerId;
        public string profileImage;
        public bool emailLeadSubscription;
        public bool emailFollowSubscription;

        /// <summary>
        /// Whether or not the user has been initialized
        /// </summary>
        public bool IsUninitialized
        {
            get
            {
                if (id == 0 && las2peerId == 0)
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