using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurndownChart3DTestRunner : MonoBehaviour
{
    public SurfacePlot surfacePlot;

    private void Awake()
    {
        if (surfacePlot == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(surfacePlot));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Vector2Int gridSize = new Vector2Int(20, 20);
            surfacePlot.GridSize = gridSize;
            float[,] heightData = new float[gridSize.x + 1, gridSize.y + 1];
            for (int y = 0; y <= gridSize.y; y++)
            {
                for (int x = 0; x <= gridSize.x; x++)
                {
                    heightData[x, y] = Mathf.Sin((float)x / gridSize.x * Mathf.PI) * Mathf.Sin((float)y / gridSize.y * Mathf.PI);
                }
            }
            DataSet dataset = new DataSet();
            dataset.Points.Add(new DataPoint(Vector3.zero, Color.red));
            dataset.XAxis = new Axis();
            dataset.YAxis = new Axis();
            dataset.ZAxis = new Axis();
            surfacePlot.DataSet = dataset;
            surfacePlot.HeightData = heightData;
        }
    }
}
