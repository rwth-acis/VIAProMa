using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IssueDataDisplay))]
public class CardSerializer : MonoBehaviour, ISerializable
{
    private const string sourceKey = "source";
    private const string issueIdKey = "issueId";
    private const string projectIdKey = "projectId";

    private IssueDataDisplay dataDisplay;

    private void Awake()
    {
        dataDisplay = GetComponent<IssueDataDisplay>();
    }

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

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Integers.Add(sourceKey, (int)dataDisplay.Content.Source);
        serializedObject.Integers.Add(issueIdKey, dataDisplay.Content.Id);
        serializedObject.Integers.Add(projectIdKey, dataDisplay.Content.ProjectId);
        return serializedObject;
    }
}
