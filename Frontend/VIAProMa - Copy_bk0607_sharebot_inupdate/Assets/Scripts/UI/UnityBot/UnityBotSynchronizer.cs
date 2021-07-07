using UnityEngine;
using Photon.Pun;
using i5.VIAProMa.Multiplayer.Avatars;
using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
using i5.VIAProMa.UI;
using i5.VIAProMa.Multiplayer.Chat;
using HoloToolkit.Unity;
using Photon.Realtime;

[RequireComponent(typeof(Animator))]
public class UnityBotSynchronizer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject unityBot;
    [SerializeField] private GameObject eve;
    [SerializeField] private GameObject chatCover;
    [SerializeField] private Interactable closeBButton, shareBotButton, shareChatBox, closeButton;

    private void Awake ()
    {
        if (photonView.IsMine)
        {
            Debug.Log(photonView.Owner.NickName);
        }
        else
        {
            unityBot.SetActive(false); // do not allow others talk to the bot
            chatCover.SetActive(true); // activate chat cover
            // disable close button
            closeButton.Enabled = false;
            // disable share button
            shareBotButton.Enabled = false;
        }
    }

    void Update()
    {
        if (string.Equals(NotificationHandler.pName, PhotonNetwork.LocalPlayer.NickName))
        {
            Debug.Log(NotificationHandler.pName + " " + PhotonNetwork.LocalPlayer.NickName);
            Share();
        }
    }

    public void ShowPlayerList()
    {
        WindowManager.Instance.PlayerList.Open(eve.transform.position * 2f, eve.transform.eulerAngles * 5);
        WindowManager.Instance.PlayerList.ListPlayer();
    }

    void Share()
    {
        Debug.Log("share: " + PhotonNetwork.LocalPlayer.NickName);
        chatCover.SetActive(false);
        shareChatBox.Enabled = false;
        unityBot.SetActive(true);
        closeButton.Enabled = false;
    }
    
}
