using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(ScatterVisualizer))]
public class ScatterSynchronizer : TransformSynchronizer
{
    private ScatterVisualizer visualizer;
    private int remoteSynchronization = 0;
    private bool initialized;

    private void Awake()
    {
        visualizer = GetComponent<ScatterVisualizer>();
    }

    public void Initial(string name)
    {
        visualizer.name = name;
        initialized = true;
        SendConfiguration();
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
        visualizer.ConfigurationChanged += OnConfigurationChanged;
    }

    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        SendConfiguration();
    }

    private async void SendConfiguration()
    {
        Debug.Log("SendConfiguration: " + visualizer.name);
        photonView.RPC("SetConfiguration", RpcTarget.All, visualizer.name);
    }

    [PunRPC]
    private async void SetConfiguration(string name)
    {
        Debug.Log("SetConfiguration: " + name);
        visualizer.name = name;
        visualizer.UpdateView();
    }

}