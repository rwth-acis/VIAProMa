using i5.ViaProMa.Visualizations.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterPlotTestRunner : MonoBehaviour
{
    //public VisualizationGridsController gridController;

    //public Vector3 size = Vector3.one;
    //public Vector3Int count = Vector3Int.one;

    //// Update is called once per frame
    //void Update()
    //{
    //    gridController.Setup(count, size);
    //}

    public i5.ViaProMa.Visualizations.Common.AxisController xAxisController;
    public i5.ViaProMa.Visualizations.Common.AxisController yAxisController;
    public i5.ViaProMa.Visualizations.Common.AxisController zAxisController;

    public float length = 1f;

    private void Start()
    {
        FloatAxis axis = new FloatAxis("Test", 0, 10);
        xAxisController.Setup(axis, length);
        yAxisController.Setup(axis, length);
        zAxisController.Setup(axis, length);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            FloatAxis axis = new FloatAxis("Test", 0, 10);
            xAxisController.Setup(axis, length);
            yAxisController.Setup(axis, length);
            zAxisController.Setup(axis, length);
        }
    }
}
