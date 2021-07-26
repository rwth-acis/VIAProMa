using i5.VIAProMa.UI.Chat;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using UnityEngine;

public class UnityBotButton : MonoBehaviourPunCallbacks
{
    [SerializeField] private Interactable uniBotButton;

    public void Update()
    {
        if (BotMenu.botIsOpen == "true")
        {
            photonView.RPC("DisableBotButton", RpcTarget.All);
            BotMenu.botIsOpen = "";
        }
        
        if (BotMenu.botIsOpen == "false")
        {
            photonView.RPC("EnableBotButton", RpcTarget.All);
            BotMenu.botIsOpen = "";
        }
    }

    [PunRPC]
    private void DisableBotButton()
    {
        uniBotButton.Enabled = false;
    }

    [PunRPC]
    private void EnableBotButton()
    {
        uniBotButton.Enabled = true;
    }
}
