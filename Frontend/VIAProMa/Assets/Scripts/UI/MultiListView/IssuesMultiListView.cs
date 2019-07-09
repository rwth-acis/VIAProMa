using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssuesMultiListView : MultiListView<Issue, IssueListViewItem>
{
    [SerializeField] private IssueListView[] issueListViews;

    protected override void Awake()
    {
        listViews = Array.ConvertAll(issueListViews, item => (ListViewController<Issue, IssueListViewItem>)item);
        base.Awake();
    }
}
