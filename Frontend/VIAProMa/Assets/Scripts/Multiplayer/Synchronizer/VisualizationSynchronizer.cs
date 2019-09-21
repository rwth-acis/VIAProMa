using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Synchronizes the title, content and color of a visualization
/// </summary>
public class VisualizationSynchronizer : MonoBehaviourPun
{
    private Visualization visualization;

    private int remoteSynchronizations = 0;
    private bool initialized = false;

    /// <summary>
    /// If true, a remote synchronization is currently taking place
    /// </summary>
    private bool RemoteSynchronizationInProgress { get => remoteSynchronizations > 0; }

    private void Awake()
    {
        visualization = GetComponent<Visualization>();
        if (visualization == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Visualization), gameObject);
        }
    }

    private void Start()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    initialized = true;
        //    await SendInitializationData();
        //}
        initialized = true;
    }

    private void OnEnable()
    {
        visualization.TitleChanged += OnTitleChanged;
        visualization.VisualizationUpdated += OnVisualizationUpdated;
    }

    private void OnDisable()
    {
        visualization.TitleChanged += OnTitleChanged;
        visualization.VisualizationUpdated += OnVisualizationUpdated;
    }

    private async void OnTitleChanged(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized || !PhotonNetwork.IsConnected)
        {
            return;
        }

        short titleId = await NetworkedStringManager.StringToId(visualization.Title);
        photonView.RPC("SetVisualizationTitle", RpcTarget.Others, titleId);
    }

    private void OnVisualizationUpdated(object sender, EventArgs e)
    {
        if (RemoteSynchronizationInProgress || !initialized || !PhotonNetwork.IsConnected)
        {
            return;
        }

        short[] projectIds = new short[visualization.ContentProvider.Issues.Count];
        short[] ids = new short[projectIds.Length];

        for (int i = 0; i < projectIds.Length; i++)
        {
            Issue issue = visualization.ContentProvider.Issues[i];
            if (issue.Source == DataSource.REQUIREMENTS_BAZAAR)
            {
                projectIds[i] = -1;
            }
            else
            {
                projectIds[i] = (short)issue.ProjectId;
            }
            ids[i] = (short)issue.Id;
        }
        photonView.RPC("SetVisualizationContent", RpcTarget.Others, projectIds, ids);
    }

    [PunRPC]
    private async void SetVisualizationTitle(short titleId)
    {
        remoteSynchronizations++;
        string title = await NetworkedStringManager.GetString(titleId);
        visualization.Title = title;
        remoteSynchronizations--;
    }

    [PunRPC]
    private async void SetVisualizationContent(short[] projectIds, short[] ids)
    {
        if (projectIds.Length != ids.Length)
        {
            Debug.LogWarning("Project ID array and issue ID array have different lengths. Could not synchronize.", gameObject);
            return;
        }

        remoteSynchronizations++;
        List<Issue> issues = new List<Issue>();
        for (int i=0;i<projectIds.Length;i++)
        {
            if (projectIds[i] == -1)
            {
                ApiResult<Issue> res = await RequirementsBazaar.GetRequirement(ids[i]);
                if (res.Successful)
                {
                    issues.Add(res.Value);
                }
            }
            else
            {
                ApiResult<Issue> res = await GitHub.GetIssue(projectIds[i], ids[i]);
                if (res.Successful)
                {
                    issues.Add(res.Value);
                }
            }
        }

        SingleIssuesProvider provider = new SingleIssuesProvider();
        provider.Issues = issues;
        visualization.ContentProvider = provider;

        remoteSynchronizations--;
    }
}
