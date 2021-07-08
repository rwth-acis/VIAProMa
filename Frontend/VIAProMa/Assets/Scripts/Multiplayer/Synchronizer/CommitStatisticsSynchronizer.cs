using System;
using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.Visualizations.CommitStatistics;
using i5.VIAProMa.WebConnection;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Synchronizes the commit statistics visualization
/// </summary>
[RequireComponent(typeof(CommitStatisticsVisualizer))]
public class CommitStatisticsSynchronizer : TransformSynchronizer
{
    private CommitStatisticsVisualizer visualizer;

    private int remoteSynchronization = 0;
    private bool initialized;

    /// <summary>
    /// Gets the CommitStatisticsVisualizer
    /// </summary>
    private void Awake()
    {
        visualizer = GetComponent<CommitStatisticsVisualizer>();
    }

    /// <summary>
    /// Checks if the client is the master client and initializes itself and other participants
    /// </summary>
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            initialized = true;
            SendConfiguration();
        }
    }

    /// <summary>
    /// Called by Photon if a player joins the room
    /// If the current client is the master client, it will send the configuration data to initialize the new client
    /// </summary>
    /// <param name="newPlayer">The player who joined the room</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            SendConfiguration();
        }
    }

    /// <summary>
    /// Called when the component becomes active
    /// Subscribes to the events of the CommitStatisticsVisualizer
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        visualizer.ConfigurationChanged += OnConfigurationChanged;
    }

    /// <summary>
    /// Called when the component becomes inactive
    /// Un-subscribes to the events of the CommitStatsticsVisualizer
    /// </summary>
    public override void OnDisable()
    {
        base.OnDisable();
        visualizer.ConfigurationChanged -= OnConfigurationChanged;

    }

    /// <summary>
    /// Called by the configuration changed event of the CommitStatisticsVisualizer
    /// </summary>
    /// <param name="sender">The sender of the event</param>
    /// <param name="e">Event arguments</param>
    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        // inform others about the changed configuration
        SendConfiguration();
    }

    /// <summary>
    /// Sends the current configuration of the visualizer to the other clients
    /// </summary>
    private async void SendConfiguration()
    {
        short ownerId = await NetworkedStringManager.StringToId(visualizer.Owner);
        short repositoryId = await NetworkedStringManager.StringToId(visualizer.Repository);
        photonView.RPC("SetConfiguration", RpcTarget.Others, ownerId, repositoryId);
    }

    /// <summary>
    /// Called remotely by clients in the room
    /// Receives the configuration for the visualizer and applies the settings
    /// </summary>
    /// <param name="ownerId">The string id of the repository owner</param>
    /// <param name="repositoryId">The string id of the repository name</param>
    [PunRPC]
    private async void SetConfiguration(short ownerId, short repositoryId)
    {
        string owner = await NetworkedStringManager.GetString(ownerId);
        string repository = await NetworkedStringManager.GetString(repositoryId);
        visualizer.Owner = owner;
        visualizer.Repository = repository;
        visualizer.UpdateView();
    }
}
