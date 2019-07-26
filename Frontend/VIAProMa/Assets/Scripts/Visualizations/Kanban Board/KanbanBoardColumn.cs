using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KanbanBoardColumnVisualController))]
public class KanbanBoardColumn : Visualization
{
    KanbanBoardColumnVisualController visualController;

    public string Title
    {
        get => visualController.Title;
        set
        {
            visualController.Title = value;
        }
    }

    private void Awake()
    {
        visualController = GetComponent<KanbanBoardColumnVisualController>();

        ContentProvider = new SingleIssuesProvider();
    }

    public override void UpdateView()
    {
        visualController.Issues = ContentProvider.Issues;
        base.UpdateView();
    }
}
