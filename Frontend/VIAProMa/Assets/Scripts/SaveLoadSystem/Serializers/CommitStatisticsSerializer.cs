using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CommitStatisticsVisualizer))]
public class CommitStatisticsSerializer : MonoBehaviour, ISerializable
{
    private CommitStatisticsVisualizer visualizer;

    private const string ownerKey = "commit_stat_owner";
    private const string repositoryKey = "commit_stat_repository";

    private void Awake()
    {
        visualizer = GetComponent<CommitStatisticsVisualizer>();
    }

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

    public SerializedObject Serialize()
    {
        SerializedObject obj = new SerializedObject();
        obj.Strings.Add(ownerKey, visualizer.Owner);
        obj.Strings.Add(repositoryKey, visualizer.Repository);
        return obj;
    }
}
