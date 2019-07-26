using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanbanBoardColumnTestRunner : MonoBehaviour
{
    public KanbanBoardColumnVisualController visualController;

    public float width = 0.5f;
    public float height = 1f;

    private void Update()
    {
        if (visualController != null)
        {
            visualController.Width = width;
            visualController.Height = height;
        }
    }
}
