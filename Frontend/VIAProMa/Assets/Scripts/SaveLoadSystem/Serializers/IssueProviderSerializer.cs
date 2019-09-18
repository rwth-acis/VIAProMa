using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueProviderSerializer : MonoBehaviour, ISerializable
{
    private const string projectIdKey = "vis_pIds";
    private const string idsKey = "vis_IDs";

    private Visualization visualization;

    private void Awake()
    {
        visualization = GetComponent<Visualization>();
        if (visualization == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Visualization), gameObject);
        }
    }

    public async void Deserialize(SerializedObject serializedObject)
    {
        if (visualization == null)
        {
            Debug.LogWarning("No visualization found. Cannot deserialize sava data", gameObject);
            return;
        }

        List<int> projectIds = SerializedObject.GetList(projectIdKey, serializedObject.Integers);
        List<int> ids = SerializedObject.GetList(idsKey, serializedObject.Integers);

        if (projectIds.Count != ids.Count)
        {
            Debug.LogWarning("Project IDs and issue ID lists have a different lenght. Cannot reconstruct visualization content.", gameObject);
            return;
        }

        List<Issue> issues = new List<Issue>();

        for (int i=0;i<projectIds.Count;i++)
        {
            if (projectIds[i] < 0)
            {
                ApiResult<Issue> networkResult = await RequirementsBazaar.GetRequirement(ids[i]);
                if (networkResult.Successful)
                {
                    issues.Add(networkResult.Value);
                }
            }
            else
            {
                ApiResult<Issue> networkResult = await GitHub.GetIssue(projectIds[i], ids[i]);
                if (networkResult.Successful)
                {
                    issues.Add(networkResult.Value);
                }
            }
        }

        visualization.ContentProvider = new SingleIssuesProvider();
        visualization.ContentProvider.Issues = issues;
    }

    public SerializedObject Serialize()
    {
        SerializedObject obj = new SerializedObject();

        if (visualization != null)
        {
            List<int> projectIds = new List<int>();
            List<int> ids = new List<int>();

            for (int i = 0; i < visualization.ContentProvider.Issues.Count; i++)
            {
                Issue issue = visualization.ContentProvider.Issues[i];

                if (issue.Source == DataSource.REQUIREMENTS_BAZAAR)
                {
                    projectIds.Add(-1);
                }
                else
                {
                    projectIds.Add(issue.ProjectId);
                }

                ids.Add(issue.Id);
            }

            SerializedObject.AddList(projectIdKey, projectIds, obj.Integers);
            SerializedObject.AddList(idsKey, ids, obj.Integers);

        }
        else
        {
            Debug.LogWarning("No visualization found. Returning empty save data.", gameObject);
        }
        return obj;
    }
}
