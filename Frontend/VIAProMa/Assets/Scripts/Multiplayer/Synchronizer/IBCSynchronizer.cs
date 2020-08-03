using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(InformationBoxConfigurator))]

public class IBCSynchronizer : TransformSynchronizer
{
    private InformationBoxConfigurator informationbox;
    private int remoteSynchronization = 0;
    private bool initialized;
    private PhotonView photonView;
    private string name;

    private void Awake()
    {
        informationbox = GetComponent<InformationBoxConfigurator>();
        photonView = GetComponent<PhotonView>();

        if (photonView != null)
        {
            name = photonView.Owner.NickName;
        }
        else
        {
            name = "Wendy";
            Debug.Log("photonview error");
        }
    }

    private void Start()
    {
        initialized = true;
    }

    public void BarchartButtonOnclick()
    {
        photonView.RPC("BarchartButton", RpcTarget.MasterClient, name);
    }

    [PunRPC]
    private void BarchartButton(string fname)
    {
        Debug.Log("ibcsynchronizer: " + fname);
        informationbox.name = fname;
        informationbox.BarchartButtonClick();
    }

    public void ScatterButtonOnclick()
    {
        photonView.RPC("ScatterButton", RpcTarget.MasterClient, name);
    }

    [PunRPC]
    private void ScatterButton(string fname)
    {
        informationbox.name = fname;
        informationbox.ScatterplotButtonClick();
    }

    public void ProgressButtonOnclick()
    {
        photonView.RPC("ProgressButton", RpcTarget.MasterClient, name);
    }
    
    [PunRPC]
    private void ProgressButton(string fname)
    {
        informationbox.name = fname;
        informationbox.ProgressBarButtonClick();
    }

}