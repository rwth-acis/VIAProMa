using UnityEngine;
using Photon.Pun;
using i5.VIAProMa.UI;
using i5.VIAProMa.Multiplayer.Chat;
using Photon.Realtime;
using i5.VIAProMa;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.UI.UnityBot;
using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(Animator))]
public class UnityBotSynchronizer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject unityBot;
    [SerializeField] private GameObject eve;
    [SerializeField] private GameObject chatCover;
    [SerializeField] private string requestPrefab;
    [SerializeField] private Interactable closeBButton, shareBot, closeButton;
    [SerializeField] private Interactable loginButton;
    public static string botOwner = "";
    public static string bOwn = "";
    public static bool isShare = false;
    public static bool isReq = false;

    private void Awake()
    {
        NoShare();
        UnityBotButton.BotOpen = true;
        UnityBotButton.BotClose = false;
        bOwn = photonView.Owner.NickName;
        //Debug.Log("bot owner: " + bOwn);
    }

    void Update()
    {
        //tell everyone the bot is open
        if (UnityBotButton.BotOpen == true)
            ToDisable(UnityBotButton.BotOpen);

        //using system notification
        if (NotificationSystem.isClicked == true)
        {
            //handler on Client's side
            if (string.Equals(ShareNRequestHandler.clName, PhotonNetwork.LocalPlayer.NickName))
            {
                Share();
                NotificationSystem.isClicked = false;
                NotificationSystem.Instance.HideMessage();
            }

            //handler on MS's side
            if (ShareNRequestHandler.reqOwner != "")
            {
                ShareBot(ShareNRequestHandler.reqOwner);
                NotificationSystem.isClicked = false;
                NotificationSystem.Instance.HideMessage();
            }
        }

        //check if permission role is stop_sharing
        if (PlayerList.isShare == true && PlayerList.role == PermissionRoles.STOP_SHARING)
        {
            StopSharing(PlayerListItem.playerName);
            PlayerList.isShare = false;
        }
    }

    private void ToDisable(bool status)
    {
        photonView.RPC("DisableBotButton", RpcTarget.Others, status);
    }

    [PunRPC]
    private void DisableBotButton(bool status)
    {
        //Debug.Log("Disable bot button");
        if (UnityBotButton.BotOpen != status)
            UnityBotButton.BotOpen = status;
    }

    private void ShareBot(string playerName)
    {
        Debug.Log("share bot");
        photonView.RPC("ShareUnityBot", RpcTarget.Others, playerName);
    }

    [PunRPC]
    private void ShareUnityBot(string playerName)
    {
        Debug.Log("ShareUnityBot: " + playerName + PhotonNetwork.LocalPlayer.NickName);

        if (string.Equals(playerName, PhotonNetwork.LocalPlayer.NickName))
        {
            Share();
        }

    }

    private void StopSharing(string playerName)
    {
        photonView.RPC("StopSharingUnityBot", RpcTarget.Others, playerName);
    }

    [PunRPC]
    private void StopSharingUnityBot(string playerName)
    {
        Debug.Log("StopSharingUnityBot");
        if (string.Equals(playerName, PhotonNetwork.LocalPlayer.NickName))
            NoShare();
    }

    private void NoShare()
    {
        if (photonView.IsMine || UserManager.Instance.UserRole == UserRoles.TUTOR)
        {
            loginButton.Enabled = true;
        }
        else
        {
            // others can see the bot but cannot talk to the bot
            unityBot.SetActive(false);
            chatCover.SetActive(true);
            // disable close bot button
            closeBButton.Enabled = false;
            loginButton.Enabled = false;
            shareBot.Enabled = false;
        }
        
    }

    public void RequestBotClick()
    {
        Debug.Log("Request Bot Click");
        botOwner = photonView.Owner.NickName;
        Debug.Log(botOwner + PhotonNetwork.LocalPlayer.NickName);
        isReq = true;
    }

    public void ShowPlayerList()
    {
        Vector3 targetPosition = eve.transform.position;
        targetPosition.z = 2f;
        targetPosition.y = 0f;
        WindowManager.Instance.PlayerList.Open(targetPosition, eve.transform.eulerAngles);
        WindowManager.Instance.PlayerList.ListPlayer();
    }

    public void LoginMenu()
    {
        Vector3 targetPosition = eve.transform.position;
        targetPosition.z = 2f;
        targetPosition.y = 0f;
        WindowManager.Instance.BotMenu.Open(targetPosition, eve.transform.eulerAngles);
    }

    public void Share()
    {
        Debug.Log("share: " + PhotonNetwork.LocalPlayer.NickName);
        //LoginMenu();
        loginButton.Enabled = true;
        chatCover.SetActive(false);
        shareBot.Enabled = false;
        closeButton.Enabled = false;
        unityBot.SetActive(true);
    }

}
