using i5.VIAProMa.UI.ListView.Core;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// Represents a requirement in the Requirements Bazaar
    /// </summary>
    public class Requirement : IListViewItemData, IUninitializable
    {
        public int id;
        public string name;
        public string description;
        public string realized; // the date when the requirement was realized
        public int projectId;
        public ReqBazUser creator;
        public int[] categories;
        public string creationDate;
        public string lastUpdatedDate;
        public int numberOfComments;
        public int numberOfAttachments;
        public int numberOfFollowers;
        public int upVotes;
        public int downVotes;
        public string userVoted;

        public bool IsRealized
        {
            get { return !string.IsNullOrEmpty(realized); }
        }

        public bool IsUninitialized
        {
            get
            {
                if (id == 0 && projectId == 0 && creator.IsUninitialized)
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