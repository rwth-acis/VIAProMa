using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NotificationHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private TMP_Text notifiLabel;
    public static string clName = "";
    private void Awake()
    {
        gameObject.SetActive(false);
        if (photonView.IsMine)
        {
            SendInvitation(PlayerListItem.playerName);
        }
    }

    public void NotificationClick()
    {
        UnityBotSynchronizer.isShare = true;
        clName = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log("NotificationClick: " + clName);
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
            notifiLabel.text = "You got an Invitation from " + photonView.Owner.NickName
                + " to see the Unity Bot. Click here to accept! The notification will disappear in 20s.";
            Destroy(gameObject, 20);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }

}
