using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkParticipantData : IListViewItemData
{
    public static Player[] Participants { get; set; }

    public NetworkParticipantData(PhotonNetwork.PlayerList list)
    {
        Participants = list;
    }
}
