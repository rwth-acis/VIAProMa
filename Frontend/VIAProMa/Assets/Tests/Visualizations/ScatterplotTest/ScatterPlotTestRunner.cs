using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterPlotTestRunner : MonoBehaviour
{
    public GridController gridController;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            gridController.Setup(new Vector2Int(5, 3), Vector2.one);
        }
    }
}
