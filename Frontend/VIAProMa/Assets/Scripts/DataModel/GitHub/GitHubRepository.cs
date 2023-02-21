using i5.VIAProMa.UI.ListView.Core;

namespace i5.VIAProMa.DataModel.GitHub
{
    /// <summary>
    /// Represents a requirement in the Requirements Bazaar
    /// </summary>
    public class GitHubRepository
    {
        private int id;
        private string name;

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }
    }
}