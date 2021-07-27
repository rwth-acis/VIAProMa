using i5.VIAProMa.Multiplayer.Chat;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace i5.VIAProMa.UI.Chat
{
    public class Bot : MonoBehaviourPun, IWindow
    {
        [SerializeField] private GameObject gameObject;
        bool IWindow.WindowEnabled 
        { 
            get; 
            set; 
        }

        bool IWindow.WindowOpen
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public event EventHandler WindowClosed;

        public void OnClick()
        {
            photonView.RPC("Close", RpcTarget.All);
        }

        [PunRPC]
        public void Close()
        {
            UnityBotButton.BotClose = true;
            Debug.Log("Close Bot " + UnityBotButton.BotOpen);
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.position = position;
            transform.eulerAngles = eulerAngles;
        }
    }
}

