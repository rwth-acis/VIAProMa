using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializes and deserializes issue card data
/// </summary>
[RequireComponent(typeof(IssueDataDisplay))]
public class CardSerializer : MonoBehaviour, ISerializable
{
    private const string sourceKey = "source";
    private const string issueIdKey = "issueId";
    private const string projectIdKey = "projectId";

    private IssueDataDisplay dataDisplay;

    /// <summary>
    /// Sets the component up
    /// </summary>
    private void Awake()
    {
        dataDisplay = GetComponent<IssueDataDisplay>();
    }

    /// <summary>
    /// Deserializes the given SerializedObject and applies its values
    /// Expects the keys "source", "issueId", "projectId" in order to load the issue
    /// </summary>
    /// <param name="serializedObject">The SerializedObject with the save data</param>
    public async void Deserialize(SerializedObject serializedObject)
    {
        DataSource source = (DataSource)serializedObject.Integers[sourceKey];
        int issueId = serializedObject.Integers[issueIdKey];
        int projectId = serializedObject.Integers[projectIdKey];
        Issue issue = null;
        ApiResult<Issue> res = null;
        switch (source)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                res = await RequirementsBazaar.GetRequirement(issueId);
                break;
            case DataSource.GITHUB:
                res = await GitHub.GetIssue(projectId, issueId);
                break;
        }
        if (res != null && res.Successful)
        {
            issue = res.Value;
        }
        dataDisplay.Setup(issue);
    }

    /// <summary>
    /// Serializes the data of the given issue card into a SerializedObject
    /// Inserts values for "source", "issueId" and "projectId"
    /// </summary>
    /// <returns>The SerializedObject with the values for the card</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Integers.Add(sourceKey, (int)dataDisplay.Content.Source);
        serializedObject.Integers.Add(issueIdKey, dataDisplay.Content.Id);
        serializedObject.Integers.Add(projectIdKey, dataDisplay.Content.ProjectId);
        return serializedObject;
    }
}
