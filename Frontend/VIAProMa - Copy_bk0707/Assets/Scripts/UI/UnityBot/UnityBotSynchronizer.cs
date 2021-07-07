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
    [SerializeField] private string requestPrefab;
    [SerializeField] private Interactable closeBButton, shareBot, closeButton;
    public static string botOwner = "";
    private bool isShare = false;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            Debug.Log(photonView.Owner.NickName);
            botOwner = photonView.Owner.NickName;
        }
        else
        {
            // do not allow others talk to the bot
            unityBot.SetActive(false);
            chatCover.SetActive(true);
            // disable close bot button
            closeBButton.Enabled = false;
        }
    }

    void Update()
    {
        if (isShare == false)
        {
            if (string.Equals(NotificationHandler.pName, PhotonNetwork.LocalPlayer.NickName))
            {
                Debug.Log(NotificationHandler.pName + " " + PhotonNetwork.LocalPlayer.NickName);
                Share();
            }
            if (string.Equals(RequestHandler.reqOwner, PhotonNetwork.LocalPlayer.NickName))
            {
                Debug.Log(RequestHandler.reqOwner + " " + PhotonNetwork.LocalPlayer.NickName);
                Share();
            }
        }

    }

    public void RequestBotClick()
    {
        Debug.Log("Request Bot Click");
        botOwner = photonView.Owner.NickName;
        Debug.Log(botOwner + PhotonNetwork.LocalPlayer.NickName);
        Vector3 targetPosition = transform.position - 1f * transform.right;
        targetPosition.y = 1f;
        PhotonNetwork.Instantiate(requestPrefab, targetPosition, Quaternion.identity, 0);
        
    }

    public void ShowPlayerList()
    {
        WindowManager.Instance.PlayerList.Open(eve.transform.position * 2f, eve.transform.eulerAngles * 5);
        WindowManager.Instance.PlayerList.ListPlayer();
    }

    public void Share()
    {
        isShare = true;
        Debug.Log("share: " + PhotonNetwork.LocalPlayer.NickName);
        chatCover.SetActive(false);
        shareBot.Enabled = false;
        closeButton.Enabled = false;
        unityBot.SetActive(true);
    }

}
