using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializes and deserialized a visualization and its content
/// </summary>
[RequireComponent(typeof(Visualization))]
public class VisualizationSerializer : MonoBehaviour, ISerializable
{
    /// <summary>
    /// Key to store and retrieve the title of the visualization
    /// </summary>
    private const string titleKey = "visualizationTitle";
    /// <summary>
    /// Key for storing the project IDs of the visualization content
    /// </summary>
    private const string projectIdKey = "vis_pIds";
    /// <summary>
    /// Key for storing the IDs of the visualization content
    /// </summary>
    private const string idsKey = "vis_IDs";

    /// <summary>
    /// The visualization to serialize/deserialize
    /// </summary>
    private Visualization visualization;

    /// <summary>
    /// Initializes the component by getting the reference to the visualization
    /// </summary>
    private void Awake()
    {
        visualization = GetComponent<Visualization>();
        if (visualization == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Visualization), gameObject);
        }
    }

    /// <summary>
    /// Deserializes the given serializedObject and applies the found relevant properties to the visualization
    /// </summary>
    /// <param name="serializedObject">A serialized object which contains the properties for the visualization</param>
    public async void Deserialize(SerializedObject serializedObject)
    {
        visualization.Title = serializedObject.Strings[titleKey];

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

        for (int i = 0; i < projectIds.Count; i++)
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

    /// <summary>
    /// Serializes the visualization into a SerializedObject
    /// </summary>
    /// <returns>A SerializedObject which contains the relevant properties of the visualization</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Strings.Add(titleKey, visualization.Title);

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

            SerializedObject.AddList(projectIdKey, projectIds, serializedObject.Integers);
            SerializedObject.AddList(idsKey, ids, serializedObject.Integers);

        }
        else
        {
            Debug.LogWarning("No visualization found. Returning empty visualization content data.", gameObject);
        }

        return serializedObject;
    }
}
