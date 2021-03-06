﻿using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.WebConnection;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    /// <summary>
    /// Synchronizes the title, content and color of a visualization
    /// </summary>
    public class VisualizationSynchronizer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private bool synchronizeColor;

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
            if (PhotonNetwork.IsMasterClient)
            {
                initialized = true;
                SendInitializationData();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            visualization.TitleChanged += OnTitleChanged;
            visualization.VisualizationUpdated += OnVisualizationUpdated;
            if (synchronizeColor)
            {
                visualization.ColorChanged += OnColorChanged;
            }
        }

        public override void OnDisable()
        {
            visualization.TitleChanged -= OnTitleChanged;
            visualization.VisualizationUpdated -= OnVisualizationUpdated;
            if (synchronizeColor)
            {
                visualization.ColorChanged -= OnColorChanged;
            }
            base.OnDisable();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                SendInitializationData();
            }
        }

        private async void SendInitializationData()
        {
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }

            short titleId = await NetworkedStringManager.StringToId(visualization.Title);
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

            photonView.RPC("Initialize", RpcTarget.Others, titleId, projectIds, ids, ConversionUtilities.ColorToVector3(visualization.Color));
            Debug.Log("Sent visualization initialization data", gameObject);
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

        private void OnColorChanged(object sender, EventArgs e)
        {
            if (RemoteSynchronizationInProgress || !initialized || !PhotonNetwork.IsConnected)
            {
                return;
            }

            photonView.RPC("SetVisualizationColor", RpcTarget.Others, ConversionUtilities.ColorToVector3(visualization.Color));
        }

        [PunRPC]
        private void Initialize(short titleId, short[] projectIds, short[] ids, Vector3 color)
        {
            Debug.Log("RPC: Initializing visualization", gameObject);
            remoteSynchronizations++;
            SetVisualizationTitle(titleId);
            SetVisualizationContent(projectIds, ids);
            SetVisualizationColor(color);
            initialized = true;
            remoteSynchronizations--;
        }

        [PunRPC]
        private async void SetVisualizationTitle(short titleId)
        {
            Debug.Log("RPC: Setting visualization title", gameObject);
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

            Debug.Log("RPC: Setting visualization content", gameObject);
            remoteSynchronizations++;
            List<Issue> issues = new List<Issue>();
            for (int i = 0; i < projectIds.Length; i++)
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

        [PunRPC]
        private void SetVisualizationColor(Vector3 color)
        {
            Debug.Log("RPC: Setting visualization color", gameObject);
            remoteSynchronizations++;
            Color convertedColor = ConversionUtilities.Vector3ToColor(color);
            visualization.Color = convertedColor;
            remoteSynchronizations--;
        }
    }
}