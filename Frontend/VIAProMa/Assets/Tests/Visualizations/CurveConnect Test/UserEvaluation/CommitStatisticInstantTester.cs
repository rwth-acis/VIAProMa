using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;

/// <summary>
/// Test script for the commit statistics visualization
/// </summary>
public class CommitStatisticInstantTester : MonoBehaviourPunCallbacks
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

    public override void OnJoinedRoom()
    {
        Debug.Log("Commit Statistcs Test");
        diagram.Size = size;
        visualizer.Owner = "rwth-acis";
        visualizer.Repository = "las2peer";
        visualizer.UpdateView();
    }
}
