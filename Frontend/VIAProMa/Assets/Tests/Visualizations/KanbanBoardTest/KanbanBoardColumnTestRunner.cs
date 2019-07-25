using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanbanBoardColumnTestRunner : MonoBehaviour
{
    public KanbanBoardColumnVisualController visualController;

    public float height = 1f;

    private void Update()
    {
        if (visualController != null)
        {
            visualController.Height = height;
        }
    }
}
