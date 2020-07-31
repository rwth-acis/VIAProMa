using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(ProgressOrganizer))]

public class POSynchronizer : TransformSynchronizer
{
    private ProgressOrganizer organizer;
    private int remoteSynchronization = 0;
    private bool initialized;

    private void Awake()
    {
        organizer = GetComponent<ProgressOrganizer>();
    }

    private void Start()
    {
        initialized = true;
    }

    public void SendClear()
    {
        photonView.RPC("myClear3", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void myClear3()
    {
        organizer.myClear();
    }

    public void NewspaceOnclick()
    {
        photonView.RPC("Newspace3", RpcTarget.MasterClient);
    }

    public void CompressOnclick()
    {
        photonView.RPC("Compress3", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void Newspace3()
    {
        organizer.Newspace();
    }

    [PunRPC]
    private void Compress3()
    {
        organizer.Compress();
    }

}