using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using UnityEngine;

public class UnityBotButton : Singleton<UnityBotButton>
{
    [SerializeField] private Interactable uniBotButton;
    public static bool BotOpen = false;
    public static bool BotClose = true;

    public void Update()
    {
 
        //Debug.Log("CheckBotStatus: " + BotOpen+BotClose);
        if (BotOpen == true & BotClose == false)
        {
            if (uniBotButton.Enabled == true)
                uniBotButton.Enabled = false;
        }
        
        if (BotClose == true & PhotonNetwork.InRoom)
        {
            if (uniBotButton.Enabled == false)
                uniBotButton.Enabled = true;
        }

    }
   
}
