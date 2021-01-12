using i5.VIAProMa.DataModel.API;

namespace i5.VIAProMa.Shelves.IssueShelf
{
    public class GitHubShelfConfiguration : IShelfConfiguration
    {
        public DataSource SelectedSource { get; private set; }

        public string Owner { get; set; }

        public string RepositoryName { get; set; }

        public bool hasAccess { get; set; }

        /// <summary>
        /// true if the owner and repository in this configuration are both non-empty
        /// </summary>
        public bool IsValidConfiguration
        {
            get
            {
                return !string.IsNullOrEmpty(Owner) && !string.IsNullOrEmpty(RepositoryName);
            }
        }

        public GitHubShelfConfiguration()
        {
            SelectedSource = DataSource.GITHUB;
        }

        public GitHubShelfConfiguration(string owner) : this()
        {
            Owner = owner;
        }

        public GitHubShelfConfiguration(string owner, string repository) : this(owner)
        {
            RepositoryName = repository;
        }
    }
}