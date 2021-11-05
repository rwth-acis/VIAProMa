using i5.VIAProMa;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.UI.UnityBot;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ShareNRequestHandler : MonoBehaviourPunCallbacks
{

    public static string clName, reqOwner = "";
    public static bool shareNreq = false;
    private void Update()
    {
       
        if (UnityBotSynchronizer.isReq == true) //true on the client's side, client request to see the bot
        {
            Debug.Log(UnityBotSynchronizer.botOwner);
            SendInvReq(UnityBotSynchronizer.botOwner, PhotonNetwork.LocalPlayer.NickName);
            UnityBotSynchronizer.isReq = false;
        }

        if (PlayerList.isShare == true) //true on the Ms' side, send invitation to client
        {
            if (!string.Equals(PlayerListItem.playerName, photonView.Owner.NickName) && PlayerList.role == PermissionRoles.SHARE)
                SendInvReq(PlayerListItem.playerName, photonView.Owner.NickName);
            PlayerList.isShare = false;
        }
    }

    public void SendInvReq(string playerName, string _pName)
    {
        Debug.Log("Sendinvitation: " + playerName);
        if (UserManager.Instance.UserRole == UserRoles.TUTOR)
            _pName = "TUTOR";
        photonView.RPC("SendNotification", RpcTarget.Others, playerName, _pName);
    }

    [PunRPC]
    private void SendNotification(string playerName, string _pName)
    {
        Debug.Log("SendNotification: " + playerName + PhotonNetwork.LocalPlayer.NickName);

        if (string.Equals(playerName, PhotonNetwork.LocalPlayer.NickName))
        {
   
            shareNreq = true;
            if (string.Equals(_pName, photonView.Owner.NickName) || _pName == "TUTOR") //view from Client, Ms or Tutor shares the bot
            {
                NotificationSystem.Instance.ShowMessage("You got an invitation from " + _pName
                + " to see the Unity Bot. Click here to accept!");
                clName = playerName;
            }

            else //view from MS, client requests to see the bot
            {
                NotificationSystem.Instance.ShowMessage("User " + _pName
                + " wants to see the Unity Bot. Click here to accept!");
                reqOwner = _pName;
            }
            
        }

    }

}
