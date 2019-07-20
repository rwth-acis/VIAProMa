using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IssueDataDisplay))]
public class CardSerializer : MonoBehaviour, ISerializable
{
    private const string sourceKey = "source";
    private const string issueIdKey = "issueId";
    private const string ownerKey = "gitHubOwner";
    private const string repositoryKey = "gitHubRepository";

    private IssueDataDisplay dataDisplay;

    private void Awake()
    {
        dataDisplay = GetComponent<IssueDataDisplay>();
    }

    public async void Deserialize(SerializedObject serializedObject)
    {
        DataSource source = (DataSource)serializedObject.Integers[sourceKey];
        int issueId = serializedObject.Integers[issueIdKey];
        Issue issue = null;
        ApiResult<Issue> res = null;
        switch (source)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                res = await RequirementsBazaar.GetRequirement(issueId);
                break;
            case DataSource.GITHUB:
                string owner = serializedObject.Strings[ownerKey];
                string repository = serializedObject.Strings[repositoryKey];
                res = await GitHub.GetIssue(owner, repository, issueId);
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
        if (dataDisplay.Content.Source == DataSource.GITHUB)
        {
            serializedObject.Strings.Add(ownerKey, dataDisplay.Content.Owner);
            serializedObject.Strings.Add(repositoryKey, dataDisplay.Content.RepositoryName);
        }
        return serializedObject;
    }
}
