using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssuesLoader : Shelf, ILoadShelf
{
    [SerializeField] private ShelfConfiguration configuration;

    [SerializeField] private Transform[] boards;

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
        if (boards.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(boards));
        }
        else
        {
            for (int i=0;i<boards.Length;i++)
            {
                if (boards[i] == null)
                {
                    SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(boards), i);
                }
            }
        }
    }

    public void LoadContent()
    {
        // load requirements from the correct project or category
        for (int i=0;i<boards.Length;i++)
        {
            if (configuration.SelectedCategory != null) // project and category were selected
            {

            }
            else if (configuration.SelectedProject != null) // just a project was selected
            {

            }
        }
    }

    private void RemoveContent()
    {
        for (int i=0;i<boards.Length;i++)
        {
            foreach(Transform obj in boards[i])
            {
                Destroy(obj);
            }
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
