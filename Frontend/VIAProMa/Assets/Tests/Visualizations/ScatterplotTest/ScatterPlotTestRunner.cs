using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterPlotTestRunner : MonoBehaviour
{
    public VisualizationGridsController gridController;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            gridController.Setup(new Vector3Int(3, 6, 2), new Vector3(1, 1,1));
        }
    }
}
