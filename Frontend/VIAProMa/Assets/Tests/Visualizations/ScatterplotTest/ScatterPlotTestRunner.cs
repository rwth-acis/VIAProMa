using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterPlotTestRunner : MonoBehaviour
{
    public VisualizationGridsController gridController;

    public Vector3 size = Vector3.one;
    public Vector3Int count = Vector3Int.one;

    // Update is called once per frame
    void Update()
    {
        gridController.Setup(count, size);
    }
}
