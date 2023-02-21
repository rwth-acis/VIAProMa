using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.DataModel.API;

namespace i5.VIAProMa.DataModel.GitHub
{
    /// <summary>
    /// Represents a requirement in the Requirements Bazaar
    /// </summary>
    public class GitHubIssue : IListViewItemData, IUninitializable
    {
        private int id;
        private int number;
        private string state;
        private string title;
        private string body;
        private GitHubUser user;
        private string created_at;
        private string updated_at;
        private string closed_at;
        private GitHubLabel[] labels;
        private GitHubUser[] assignees;
        private bool locked;

        public int getId()
        {
            return id;
        }

        public int getNumber()
        {
            return number;
        }

        public string getState()
        {
            return state;
        }

        public string getTitle()
        {
            return title;
        }

        public string getBody()
        {
            return body;
        }

        public GitHubUser getUser()
        {
            return user;
        }

        public string getCreated_at() { return created_at; }

        public string getUpdated_at() { return updated_at; }

        public string getClosed_at() { return closed_at; }

        public GitHubLabel[] getLabels()
        {
            return labels;
        }

        public GitHubUser[] getAssignees()
        {
            return assignees;
        }

        public bool isLocked()
        {
            return locked;
        }

        public IssueStatus getIssueStatus()
        {
            if (state == "closed")
            {
                return IssueStatus.CLOSED;
            }
            else
            {
                if (assignees != null && assignees.Length > 0)
                {
                    return IssueStatus.IN_PROGRESS;
                }
                else
                {
                    return IssueStatus.OPEN;
                }
            }
        }

        public bool IsUninitialized
        {
            get
            {
                if (id == 0 && user.IsUninitialized)
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