using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(ScatterOrganizer))]

public class SOSynchronizer : TransformSynchronizer
{
    private ScatterOrganizer organizer;
    private int remoteSynchronization = 0;
    private bool initialized;

    private void Awake()
    {
        organizer = GetComponent<ScatterOrganizer>();
    }

    private void Start()
    {
        initialized = true;
    }

    public void SendClear()
    {
        photonView.RPC("myClear2", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void myClear2()
    {
        organizer.myClear();
    }

    public void NewspaceOnclick()
    {
        photonView.RPC("Newspace2", RpcTarget.MasterClient);
    }

    public void CompressOnclick()
    {
        photonView.RPC("Compress2", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void Newspace2()
    {
        organizer.Newspace();
    }

    [PunRPC]
    private void Compress2()
    {
        organizer.Compress();
    }

}