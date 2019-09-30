using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the save-load data for the commit statistics visualization
/// </summary>
[RequireComponent(typeof(CommitStatisticsVisualizer))]
public class CommitStatisticsSerializer : MonoBehaviour, ISerializable
{
    private CommitStatisticsVisualizer visualizer;

    private const string ownerKey = "commit_stat_owner";
    private const string repositoryKey = "commit_stat_repository";

    /// <summary>
    /// Gets the visualizer script
    /// </summary>
    private void Awake()
    {
        visualizer = GetComponent<CommitStatisticsVisualizer>();
    }

    /// <summary>
    /// Called to deserialize a given SerializedObject and apply its settings to the component
    /// </summary>
    /// <param name="serializedObject">The save data which should be deserialized</param>
    public void Deserialize(SerializedObject serializedObject)
    {
        string owner = SerializedObject.TryGet(ownerKey, serializedObject.Strings, gameObject, out bool foundOwner);
        if (foundOwner)
        {
            visualizer.Owner = owner;
        }
        string repository = SerializedObject.TryGet(repositoryKey, serializedObject.Strings, gameObject, out bool foundRepository);
        if (foundRepository)
        {
            visualizer.Repository = repository;
        }

        if (foundOwner && foundRepository)
        {
            visualizer.UpdateView();
        }
    }

    /// <summary>
    /// Serializes the settings of the CommitStatisticVisualizer
    /// </summary>
    /// <returns>The save data for the CommitStatisticVisualizer</returns>
    public SerializedObject Serialize()
    {
        SerializedObject obj = new SerializedObject();
        obj.Strings.Add(ownerKey, visualizer.Owner);
        obj.Strings.Add(repositoryKey, visualizer.Repository);
        return obj;
    }
}
