using i5.VIAProMa.UI.Chat;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
public class RequestHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private TMP_Text notifiLabel;
    public static string reqOwner = "";

    private void Awake()
    {
        gameObject.SetActive(false);
        if (photonView.IsMine)
        {
            Debug.Log("Photon view owner of request: " + photonView.Owner.NickName);
            SendRequest(UnityBotSynchronizer.botOwner);
        }
    }

    public void NotificationClick()
    {
        UnityBotSynchronizer.isReq = true;
        reqOwner = photonView.Owner.NickName;
        Debug.Log("NotificationClick: " + reqOwner);
        Destroy(gameObject);

    }

    public void SendRequest(string playerName)
    {
        Debug.Log("SendRequest to bot owner: " + playerName);
        photonView.RPC("SendRequestToOwner", RpcTarget.Others, playerName);
    }

    [PunRPC]
    private void SendRequestToOwner(string playerName)
    {
        Debug.Log("SendRequestToOwner: " + playerName + PhotonNetwork.NickName);

        if (string.Equals(playerName, PhotonNetwork.LocalPlayer.NickName))
        {
            gameObject.SetActive(true);
            notifiLabel.text = "User " + photonView.Owner.NickName 
                + " wants to see the Unity Bot. Click here to accept! The notification will disappear in 20s.";
            Destroy(gameObject, 20);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

}