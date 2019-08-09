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
            surfacePlot.GridSize = new Vector2Int(5, 5);
            DataSet ds = new DataSet();
            ds.Points.Add(new DataPoint(Vector3.one, Color.red));
            surfacePlot.DataSet = ds;
        }
    }
}
