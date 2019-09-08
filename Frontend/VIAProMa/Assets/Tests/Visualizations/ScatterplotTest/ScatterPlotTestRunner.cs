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

    public i5.ViaProMa.Visualizations.Common.Diagram diagram;
    public Vector3 size;

    private void Start()
    {
        diagram.Size = size;
        diagram.UpdateGridAxes();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            diagram.Size = size;
            diagram.UpdateGridAxes();
        }
    }
}
