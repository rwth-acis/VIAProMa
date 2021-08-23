using UnityEngine;
using Photon.Pun;
using i5.VIAProMa.Visualizations.CommitStatistics;
using i5.VIAProMa.Visualizations.Common;

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

    private Diagram diagram;

    /// <summary>
    /// Gets the diagram component of the visualization
    /// </summary>
    private void Awake()
    {
        diagram = visualizer.GetComponent<Diagram>();
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
