using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using System.Collections.Generic;

namespace i5.VIAProMa.Visualizations.Competence
{
    public class UserScore : IListViewItemData
    {
        public User User { get; private set; }

        private float creatorWeight;
        private float commenterWeight;
        private float developerWeight;
        private float closedDeveloperWeight;

        private List<Issue> createdIssues;
        private List<Issue> commentedIssues;
        private List<Issue> developedIssues;
        private List<Issue> closedDevelopedIssues;

        public float Score
        {
            get
            {
                return creatorWeight * createdIssues.Count + commenterWeight * commentedIssues.Count
                    + developerWeight * developedIssues.Count + closedDeveloperWeight * closedDevelopedIssues.Count;
            }
        }

        public UserScore(User user, float creatorWeight, float commenterWeight, float developerWeight, float closedDeveloperWeight)
        {
            User = user;
            this.creatorWeight = creatorWeight;
            this.commenterWeight = commenterWeight;
            this.developerWeight = developerWeight;
            this.closedDeveloperWeight = closedDeveloperWeight;

            createdIssues = new List<Issue>();
            commentedIssues = new List<Issue>();
            developedIssues = new List<Issue>();
            closedDevelopedIssues = new List<Issue>();
        }

        public int CreatedIssuesCount { get => createdIssues.Count; }
        public int CommentedIssuesCount { get => commentedIssues.Count; }
        public int DevelopedIssuesCount { get => developedIssues.Count; }
        public int ClosedIssuesCount { get => closedDevelopedIssues.Count; }

        public void AddCreatedIssue(Issue issue)
        {
            createdIssues.Add(issue);
        }

        public void AddCommentedIssue(Issue issue)
        {
            commentedIssues.Add(issue);
        }

        public void AddDevelopedIssue(Issue issue)
        {
            developedIssues.Add(issue);
        }

        public void AddClosedDevelopedIssue(Issue issue)
        {
            closedDevelopedIssues.Add(issue);
        }
    }
}