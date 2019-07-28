using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KanbanBoardColumnVisualController))]
public class KanbanBoardColumn : Visualization
{
    protected override void Awake()
    {
        base.Awake();

        visualController = GetComponent<KanbanBoardColumnVisualController>();

        ContentProvider = new SingleIssuesProvider();
    }

    public override void UpdateView()
    {
        ((KanbanBoardColumnVisualController)visualController).Issues = ContentProvider.Issues;
        base.UpdateView();
    }
}
