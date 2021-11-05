using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.Utilities;
using i5.Toolkit.Core.OpenIDConnectClient;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;
using i5.VIAProMa.Login;
using Photon.Pun;

namespace i5.VIAProMa.UI.Chat
{
    public class BotMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private string botPrefab;
        [SerializeField] GameObject notification;
        public bool isSubscribedToOidc = false;
        public static string botIsOpen = "";

        public bool WindowEnabled 
        { 
            get; 
            set; 
        }

        public bool WindowOpen
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public event EventHandler WindowClosed;

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

        public void Close()
        {
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Login()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (!isSubscribedToOidc)
            {
                ServiceManager.GetService<SlackOidcService>().LoginCompleted += LoginScript_LoginCompleted;
                isSubscribedToOidc = true;
            }
            ServiceManager.GetService<SlackOidcService>().SlackOpenLoginPage();
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WSA
            Close();
            if (UnityBotButton.BotOpen == false)
            {
                Debug.Log("Show Bot");
                OpenUniBot();
            }  
#endif
        }

        public void OpenUniBot()
        {
            //WindowManager.Instance.BotMenu.Close();
            Debug.Log("Show Bot" + UnityBotButton.BotOpen + UnityBotButton.BotClose);
            Vector3 targetPosition = transform.position - 1f * transform.right;
            targetPosition.z = 3f;
            targetPosition.y = -0.8f;
            PhotonNetwork.Instantiate(botPrefab, targetPosition, Quaternion.Euler(0, 90, 0), 0);
        }

        private void LoginScript_LoginCompleted(object sender, System.EventArgs e)
        {
            Debug.Log("Login completed", this);
            Debug.Log(ServiceManager.GetService<SlackOidcService>().AccessToken, this);
            //Hide BotMenu
            Close();
            if (UnityBotButton.BotOpen == false & UnityBotButton.BotClose == true)
            {
                OpenUniBot();
            }           
            ServiceManager.GetService<SlackOidcService>().LoginCompleted -= LoginScript_LoginCompleted;
            isSubscribedToOidc = false;
            //IUserInfo userInfo = await ServiceManager.GetService<SlackOidcService>().GetUserDataAsync();
        }
    }

}

