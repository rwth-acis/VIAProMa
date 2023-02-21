using i5.VIAProMa.UI.ListView.Core;

namespace i5.VIAProMa.DataModel.GitHub
{
    /// <summary>
    /// Represents a requirement in the Requirements Bazaar
    /// </summary>
    public class GitHubLabel : IListViewItemData
    {
        private long id;
        private string nodeId;
        private string name;
        private string description;
        private string color;

        public long getId()
        {
            return id;
        }

        public string getNodeId()
        {
            return nodeId;
        }

        public string getName()
        {
            return name;
        }

        public string getDescription()
        {
            return description;
        }

        public string getColor()
        {
            return color;
        }
    }
}