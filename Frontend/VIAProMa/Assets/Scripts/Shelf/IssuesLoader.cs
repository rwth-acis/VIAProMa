using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssuesLoader : Shelf, ILoadShelf
{
    [SerializeField] private InputField searchField;
    [Header("References")]
    [SerializeField] private ShelfConfigurationMenu configurationMenu;
    [SerializeField] private IssuesMultiListView issuesMultiListView;

    public MessageBadge MessageBadge { get => messageBadge; }

    private Issue[] issues;
    private Issue[] nextIssues;

    protected override void Awake()
    {
        base.Awake();
        if (configurationMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationMenu));
        }
        if (issuesMultiListView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(issuesMultiListView));
        }
        if (searchField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchField));
        }
        upButton.Enabled = false;
    }

    public void LoadContent()
    {
        switch (configurationMenu.ShelfConfiguration.SelectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                LoadRequirements();
                break;
            case DataSource.GITHUB:
                LoadGitHubIssues();
                break;
        }

        CheckControls();
    }

    private async void LoadRequirements()
    {
        if (!configurationMenu.ShelfConfiguration.IsValidConfiguration)
        {
            issuesMultiListView.Items = new List<Issue>();
            return;
        }
        ReqBazShelfConfiguration reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
        messageBadge.ShowLoadMessage();
        ApiResult<Issue[]> apiResult = null;
        // load requirements from the correct project or category
        if (reqBazShelfConfiguration.SelectedCategory != null) // project and category were selected
        {
            apiResult = await RequirementsBazaar.GetRequirementsInCategory(reqBazShelfConfiguration.SelectedCategory.id, page, issuesMultiListView.numberOfItemsPerListView * issuesMultiListView.NumberOfListViews, searchField.Text);
        }
        else if (reqBazShelfConfiguration.SelectedProject != null) // just a project was selected
        {
            apiResult = await RequirementsBazaar.GetRequirementsInProject(reqBazShelfConfiguration.SelectedProject.id, page, issuesMultiListView.numberOfItemsPerListView * issuesMultiListView.NumberOfListViews, searchField.Text);
        }
        else
        {
            issuesMultiListView.Clear();
        }
        messageBadge.DoneProcessing();
        if (apiResult != null) // web request was made
        {
            if (apiResult.HasError)
            {
                messageBadge.ShowMessage(apiResult.ResponseCode);
            }
            else
            {
                List<Issue> items = new List<Issue>(apiResult.Value);
                issuesMultiListView.Items = items;
            }
        }
    }

    private async void LoadGitHubIssues()
    {
        if (!configurationMenu.ShelfConfiguration.IsValidConfiguration)
        {
            issuesMultiListView.Items = new List<Issue>();
            return;
        }
        GitHubShelfConfiguration gitHubShelfConfiguration = (GitHubShelfConfiguration)configurationMenu.ShelfConfiguration;
        messageBadge.ShowProcessing();
        ApiResult<Issue[]> apiResult = await GitHub.GetIssuesInRepository("", "");
        messageBadge.DoneProcessing();
        if (apiResult.HasError)
        {
            messageBadge.ShowMessage(apiResult.ResponseCode);
        }
        else
        {
            List<Issue> items = new List<Issue>(apiResult.Value);
            issuesMultiListView.Items = items;
        }
    }

    public override void ScrollUp()
    {
        base.ScrollUp();
        LoadContent();
    }

    public override void ScrollDown()
    {
        base.ScrollDown();
        LoadContent();
    }
}
