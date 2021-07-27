using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class UnityBotButton : Singleton<UnityBotButton>
{
    [SerializeField] private Interactable uniBotButton;
    public static bool BotOpen = false;
    public static bool BotClose = false;

    public void Update()
    {
        //Debug.Log("CheckBotStatus: " + BotOpen);
        if (BotOpen == true & BotClose == false)
        {
            if (uniBotButton.Enabled == true)
                uniBotButton.Enabled = false;
        }
        
        if (BotClose == true)
        {
            if (uniBotButton.Enabled == false)
                uniBotButton.Enabled = true;
        }
    }
   
}
