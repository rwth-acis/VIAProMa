using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(CommitStatisticsVisualizer))]
public class CommitStatisticsSynchronizer : TransformSynchronizer
{
    private CommitStatisticsVisualizer visualizer;

    private int remoteSynchronization = 0;
    private bool initialized;

    private void Awake()
    {
        visualizer = GetComponent<CommitStatisticsVisualizer>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            initialized = true;
            SendConfiguration();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
        {
            SendConfiguration();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        visualizer.ConfigurationChanged += OnConfigurationChanged;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        visualizer.ConfigurationChanged -= OnConfigurationChanged;

    }

    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        SendConfiguration();
    }

    private async void SendConfiguration()
    {
        short ownerId = await NetworkedStringManager.StringToId(visualizer.Owner);
        short repositoryId = await NetworkedStringManager.StringToId(visualizer.Repository);
        photonView.RPC("SetConfiguration", RpcTarget.Others, ownerId, repositoryId);
    }

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
