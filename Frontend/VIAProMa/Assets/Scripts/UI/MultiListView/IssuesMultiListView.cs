using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.UI.ListView.Issues;
using System;
using UnityEngine;

namespace i5.VIAProMa.UI.MultiListView.Core
{
    public class IssuesMultiListView : MultiListView<Issue, IssueListViewItem>
    {
        [SerializeField] private IssueListView[] issueListViews;

        protected override void Awake()
        {
            listViews = Array.ConvertAll(issueListViews, item => (ListViewController<Issue, IssueListViewItem>)item);
            base.Awake();
        }
    }
}