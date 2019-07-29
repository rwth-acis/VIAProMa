using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationShelf : Shelf
{
    public List<GameObject> visualizationWidgets;
    public int objectsPerBoard = 3;
    public int boards = 5;
    public float boardLength = 0.8f;

    [SerializeField] private GridObjectCollection shelfGrid;

    private List<GameObject> widgetInstances;    

    protected override void Awake()
    {
        base.Awake();
        if (shelfGrid == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(shelfGrid));
        }

        widgetInstances = new List<GameObject>();

        for (int i=0;i<visualizationWidgets.Count;i++)
        {
            GameObject instance = Instantiate(visualizationWidgets[i], shelfGrid.transform);
            widgetInstances.Add(instance);
        }

        DisplayWidgets();
    }

    private void DisplayWidgets()
    {
        shelfGrid.Rows = boards;
        shelfGrid.CellWidth = boardLength / objectsPerBoard;

        int startIndex = page * boards * objectsPerBoard;
        int endIndex = startIndex + boards * objectsPerBoard - 1;
        ActivateRange(startIndex, endIndex);

        shelfGrid.UpdateCollection();
    }

    private void ActivateRange(int startIndex, int endIndex)
    {
        for (int i=0;i<widgetInstances.Count;i++)
        {
            bool isActive = (i >= startIndex && i <= endIndex);
            widgetInstances[i].SetActive(isActive);
        }
    }

    public override void ScrollDown()
    {
        base.ScrollDown();
        DisplayWidgets();
    }

    public override void ScrollUp()
    {
        base.ScrollUp();
        DisplayWidgets();
    }
}
