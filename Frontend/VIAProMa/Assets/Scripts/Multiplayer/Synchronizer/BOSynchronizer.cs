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

    public void SendClear()
    {
        //photonView.RPC("myClear1", RpcTarget.MasterClient);
        organizer.myClear();
    }

    /*
    [PunRPC]
    private void myClear1()
    {
        organizer.myClear();
    }
    */

    public void NewspaceOnclick()
    {
        photonView.RPC("Newspace1", RpcTarget.MasterClient);
    }

    public void CompressOnclick()
    {
        photonView.RPC("Compress1", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void Newspace1()
    {
        organizer.Newspace();
    }

    [PunRPC]
    private void Compress1()
    {
        organizer.Compress();
    }

}