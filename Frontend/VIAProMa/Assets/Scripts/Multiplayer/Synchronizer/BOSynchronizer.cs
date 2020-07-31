using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(BarchartOrganizer))]

public class BOSynchronizer : TransformSynchronizer
{
    private BarchartOrganizer organizer;
    private int remoteSynchronization = 0;
    private bool initialized;

    private void Awake()
    {
        organizer = GetComponent<BarchartOrganizer>();
    }

    private void Start()
    {
        initialized = true;
    }

    public void NewspaceOnclick()
    {
        photonView.RPC("Newspace", RpcTarget.MasterClient);
    }

    public void CompressOnclick()
    {
        photonView.RPC("Compress", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void Newspace()
    {
        organizer.Newspace();
    }

    [PunRPC]
    private void Compress()
    {
        organizer.Compress();
    }

}