using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(InformationButtonConfigurator))]
public class InformationButtonSynchronizer : TransformSynchronizer
{
    private InformationButtonConfigurator InformationButton;
    private bool initialized;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        InformationButton = GetComponent<InformationButtonConfigurator>();
    }

    private void Start()
    {
        if (photonView.Owner.NickName == PhotonNetwork.NickName)
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

    private void OnConfigurationChanged(object sender, EventArgs e)
    {
        SendConfiguration();
    }

    private async void SendConfiguration()
    {
        photonView.RPC("SetConfiguration", RpcTarget.All, InformationButton.status);
    }

    [PunRPC]
    private async void SetConfiguration(bool status)
    {
        Debug.Log("button status, SetConfiguration: " + status);
        InformationButton.status = status;
        InformationButton.Update();
    }

}