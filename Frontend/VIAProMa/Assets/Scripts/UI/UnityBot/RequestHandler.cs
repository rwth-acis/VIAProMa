using i5.VIAProMa.UI.Chat;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RequestHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameObject;
    public static string reqOwner = "";
    private void Awake()
    {
        if (photonView.IsMine)
        {
            gameObject.SetActive(false);
            Debug.Log("Photon view owner of request: " + photonView.Owner.NickName);
            SendRequest(UnityBotSynchronizer.botOwner);
        }

    }

    public void NotificationClick()
    {
        //UnityBotSynchronizer.isReq = true;
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
        Debug.Log("SendRequestToOwner: " + playerName + PhotonNetwork.LocalPlayer.NickName);

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
