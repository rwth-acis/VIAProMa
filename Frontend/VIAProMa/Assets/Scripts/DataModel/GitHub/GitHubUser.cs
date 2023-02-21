using i5.VIAProMa.UI.ListView.Core;

namespace i5.VIAProMa.DataModel.GitHub
{
    /// <summary>
    /// Represents a requirement in the Requirements Bazaar
    /// </summary>
    public class GitHubUser : IListViewItemData, IUninitializable
    {
        private string login;
        private int id;
        private string node_id;
        private string avatar_url;
        private string email;

        public string getLogin()
        {
            return login;
        }

        public int getId()
        {
            return id;
        }

        public string getNode_id()
        {
            return node_id;
        }

        public string getAvatar_url()
        {
            return avatar_url;
        }

        public string getEmail()
        {
            return email;
        }

        public bool IsUninitialized
        {
            get
            {
                if (id == 0)
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