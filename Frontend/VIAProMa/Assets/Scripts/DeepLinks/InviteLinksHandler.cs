﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class InviteLinksHandler : MonoBehaviour
{
    /// <summary>
    /// Invite Link Handler which handles the reception of Invite Links at runtime
    /// </summary>
    

    public string decodeLink (string deeplink)
    {
        if(deeplink.IndexOf('=')== -1)
        {
            Debug.Log("Fehlerhafter DeepLink");
            return null;
        } 
        else
        {
            return deeplink.Substring(deeplink.IndexOf('=')+1);
        }

    }

    public void JoinByDeeplink(string deeplink)
    {
        string roomname = decodeLink(deeplink);
        PhotonNetwork.JoinRoom(roomname);
    }

}
