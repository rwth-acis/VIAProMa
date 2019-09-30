using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test script for the commit statistics visualization
/// </summary>
public class CommitStatisticsTester : MonoBehaviour
{
    /// <summary>
    /// Reference to the commit statistic visualization
    /// </summary>
    public CommitStatisticsVisualizer visualizer;

    /// <summary>
    /// The size of the diagram
    /// </summary>
    public Vector3 size = Vector3.one;

    private i5.ViaProMa.Visualizations.Common.Diagram diagram;

    /// <summary>
    /// Gets the diagram component of the visualization
    /// </summary>
    private void Awake()
    {
        diagram = visualizer.GetComponent<i5.ViaProMa.Visualizations.Common.Diagram>();
    }

    /// <summary>
    /// If the user presses F5, the data of las2peer are loaded in the visualization for testing purposes
    /// </summary>
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
