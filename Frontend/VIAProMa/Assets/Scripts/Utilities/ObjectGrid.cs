using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
    public int Columns { get; set; }

    public Vector2 CellSize { get; set; }

    public bool Centered { get; set; }

    private void Awake()
    {
    }

    public void UpdateGrid()
    {
        Vector2 offset = Vector2.zero;
        if (Centered)
        {
            offset.x = (Columns - 1) * CellSize.x / 2f;
            int rows = Mathf.CeilToInt(transform.childCount / Columns);
            offset.y = -(rows-1) * CellSize.y / 2f;
        }

        int row, column;
        for (int i = 0; i < transform.childCount; i++)
        {
            row = i / Columns;
            column = i % Columns;
            transform.GetChild(i).localPosition = new Vector3(
                CellSize.x * column - offset.x,
                CellSize.y * (-row) - offset.y,
                0f);
        }
    }
}
