﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IssueDataDisplay))]
public class CardSerializer : MonoBehaviour, ISerializable
{
    private const string sourceKey = "source";
    private const string issueIdKey = "issueId";

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
        if (source == DataSource.REQUIREMENTS_BAZAAR)
        {
            ApiResult<Issue> res = await RequirementsBazaar.GetRequirement(issueId);
            if (res.Successful)
            {
                issue = res.Value;
            }
        }
        dataDisplay.Setup(issue);
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Integers.Add(sourceKey, (int)dataDisplay.Content.Source);
        serializedObject.Integers.Add(issueIdKey, dataDisplay.Content.Id);
        return serializedObject;
    }
}
