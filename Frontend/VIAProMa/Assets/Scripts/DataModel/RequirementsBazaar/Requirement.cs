using i5.VIAProMa.UI.ListView.Core;
using UnityEngine;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    /// <summary>
    /// Represents a requirement in the Requirements Bazaar
    /// </summary>
    public class Requirement : IListViewItemData, IUninitializable
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private string realized; // the date when the requirement was realized
        [SerializeField] private int projectId;
        [SerializeField] private ReqBazUser creator;
        [SerializeField] private int[] categories;
        [SerializeField] private string[] tags; //Updated
        [SerializeField] private string creationDate;
        [SerializeField] private string lastUpdatedDate;
        [SerializeField] private ReqBazUser lastUpdatingUser; //Updated
        [SerializeField] private ReqBazUser lastActivityUser; //Updated
        [SerializeField] private string lastActivity; //Updated
        [SerializeField] private int numberOfComments;
        [SerializeField] private int numberOfAttachments;
        [SerializeField] private int numberOfFollowers;
        [SerializeField] private int upVotes;
        [SerializeField] private int downVotes;
        //public Context userContext;
        //public Context _context;
        [SerializeField] private string userVoted;

        [SerializeField] private Contributors contributors;

        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
        public string Realized { get => realized; } // the date when the requirement was realized
        public int ProjectId { get => projectId; }
        public ReqBazUser Creator { get => creator; }
        public int[] Categories { get => categories; }
        public string[] Tags { get => tags; } //Updated
        public string CreationDate { get => creationDate; }
        public string LastUpdatedDate { get => lastUpdatedDate; }
        public ReqBazUser LastUpdatingUser { get => lastUpdatingUser; } //Updated
        public ReqBazUser LastActivityUser { get => lastActivityUser; } //Updated
        public string LastActivity { get => lastActivity; } //Updated
        public int NumberOfComments { get => numberOfComments; }
        public int NumberOfAttachments { get => numberOfAttachments; }
        public int NumberOfFollowers { get => numberOfFollowers; }
        public int UpVotes { get => upVotes; }
        public int DownVotes { get => downVotes; }
        //public Context userContext;
        //public Context _context;
        public string UserVoted { get => userVoted; }

        public Contributors Contributors { get => contributors; set => contributors = value; }

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public string getDescription()
        {
            return description;
        }

        public string getRealized()
        {
            return realized;
        }

        public int getProjectId()
        {
            return projectId;
        }

        public ReqBazUser getCreator()
        {
            return creator;
        }

        public int[] getCategories()
        {
            return categories;
        }

        public string[] getTags()
        {
            return tags;
        }

        public string getCreationDate()
        {
            return creationDate;
        }

        public string getLastUpdatedDate()
        {
            return lastUpdatedDate;
        }

        public ReqBazUser getLastUpdatingUser()
        {
            return lastUpdatingUser;
        }

        public ReqBazUser getLastActivityUser()
        {
            return lastActivityUser;
        }

        public string getLastActivity()
        {
            return lastActivity;
        }

        public int getNumberOfComments()
        {
            return numberOfComments;
        }

        public int getNumberOfAttachments()
        {
            return numberOfAttachments;
        }

        public int getNumberOfFollowers()
        {
            return numberOfFollowers;
        }

        public int getUpVotes()
        {
            return upVotes;
        }

        public int getDownVotes()
        {
            return downVotes;
        }

        public string getUserVoted()
        {
            return userVoted;
        }

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