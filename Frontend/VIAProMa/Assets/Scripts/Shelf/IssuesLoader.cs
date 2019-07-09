using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssuesLoader : Shelf, ILoadShelf
{
    [SerializeField] private ShelfConfiguration configuration;
    [SerializeField] private IssuesMultiListView issuesMultiListView;

    public MessageBadge MessageBadge { get => messageBadge; }

    private Issue[] issues;
    private Issue[] nextIssues;

    private int page;

    protected override void Awake()
    {
        base.Awake();
        if (configuration == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configuration));
        }
        if(issuesMultiListView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(issuesMultiListView));
        }
    }

    public void LoadContent()
    {
        switch (configuration.SelectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                LoadRequirements();
                break;
            case DataSource.GITHUB:
                break;
        }
    }

    private void LoadRequirements()
    {
        // load requirements from the correct project or category
        if (configuration.SelectedCategory != null) // project and category were selected
        {
        }
        else if (configuration.SelectedProject != null) // just a project was selected
        {

        }
        else
        {
            issuesMultiListView.Clear();
        }
    }

    public void ScrollUp()
    {
        page++;
    }

    public void ScrollDown()
    {
        page--;
    }
}
