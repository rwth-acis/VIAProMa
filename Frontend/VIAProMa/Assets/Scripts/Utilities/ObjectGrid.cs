using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
    [SerializeField] private bool centered;

    public int Columns { get; set; }

    public Vector2 CellSize { get; set; }

    public bool Centered { get => centered; set => centered = value; }

    public void UpdateGrid()
    {
        int activeChildCount = 0;
        foreach(Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        Vector2 offset = Vector2.zero;
        if (Centered)
        {
            offset.x = (Columns - 1) * CellSize.x / 2f;
            int rows = Mathf.CeilToInt((float)activeChildCount / Columns);
            offset.y = -(rows-1) * CellSize.y / 2f;
        }

        int row, column;
        int iForActiveObjects = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            row = iForActiveObjects / Columns;
            column = iForActiveObjects % Columns;
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                child.localPosition = new Vector3(
                    CellSize.x * column - offset.x,
                    CellSize.y * (-row) - offset.y,
                    0f);

                iForActiveObjects++;
            }
        }
    }
}
