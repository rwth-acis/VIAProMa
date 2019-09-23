using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommitStatisticsTester : MonoBehaviour
{
    public CommitStatisticsVisualizer visualizer;

    public Vector3 size = Vector3.one;

    private i5.ViaProMa.Visualizations.Common.Diagram diagram;

    private void Awake()
    {
        diagram = visualizer.GetComponent<i5.ViaProMa.Visualizations.Common.Diagram>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("Commit Statistcs Test");
            diagram.Size = size;
            visualizer.Owner = "rwth-acis";
            visualizer.Repository = "las2peer";
            visualizer.UpdateView();
        }
    }
}
