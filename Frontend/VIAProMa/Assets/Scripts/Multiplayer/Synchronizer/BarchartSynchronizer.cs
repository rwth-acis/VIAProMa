using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(BarchartVisualizer))]
public class BarchartSynchronizer : TransformSynchronizer
{
    private BarchartVisualizer visualizer;

    private int remoteSynchronization = 0;
    private bool initialized;

    private void Awake()
    {
        visualizer = GetComponent<BarchartVisualizer>();
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
        visualizer.ConfigurationChanged += OnConfigurationChanged;
    }

    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        SendConfiguration();
    }

    private async void SendConfiguration()
    {
        //short nameId = await NetworkedStringManager.StringToId(visualizer.name);
        Debug.Log("SendConfiguration: " + visualizer.name);
        photonView.RPC("SetConfiguration", RpcTarget.All, visualizer.name);
    }

    [PunRPC]
    private async void SetConfiguration(string name)
    {
        //string name = await NetworkedStringManager.GetString(nameId);
        Debug.Log("SetConfiguration: " + name);
        visualizer.name = name;
        visualizer.UpdateView();
    }
}