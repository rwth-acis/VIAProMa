using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NotificationHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameObject;
    public static string pName = "";
    private void Awake()
    {
        if (photonView.IsMine)
        {
            gameObject.SetActive(false);
            SendInvitation(PlayerListItem.playerName);
        }
        
    }

    public void NotificationClick()
    {
        pName = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log("NotificationClick: " + pName);
        Destroy(gameObject);
        
    }

    public void SendInvitation(string playerName)
    {
        Debug.Log("Sendinvitation: " + playerName);
        photonView.RPC("SendNotification", RpcTarget.Others, playerName);
    }

    [PunRPC]
    private void SendNotification(string playerName)
    {
        Debug.Log("SendNotification: " + playerName + PhotonNetwork.LocalPlayer.NickName);
        
        if (string.Equals(playerName, PhotonNetwork.LocalPlayer.NickName))
        {
            gameObject.SetActive(true);
            Destroy(gameObject, 20);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

}
