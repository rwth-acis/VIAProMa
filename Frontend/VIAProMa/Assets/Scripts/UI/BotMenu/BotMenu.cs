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
        [SerializeField] private Interactable loginButton;
        public bool isSubscribedToOidc = false;

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

        private void Awake()
        {
           
            if (loginButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loginButton));
            }
           
        }
        public void Open()
        {
            gameObject.SetActive(true);
            //NotificationSystem.Instance.CanShowMessages = false;
            //NotificationSystem.Instance.HideMessage();
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
            /*if (NotificationSystem.Instance != null)
            {
                NotificationSystem.Instance.CanShowMessages = true;
            }*/
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Login()
        {
            if (!isSubscribedToOidc)
            {
                //ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += OpenIDConnectTester_LoginCompleted;
                ServiceManager.GetService<SlackOidcService>().LoginCompleted += LoginScript_LoginCompleted;
                isSubscribedToOidc = true;
            }
            ServiceManager.GetService<SlackOidcService>().OpenLoginPage();
        }

        public void CloseBotMenu()
        {
            WindowManager.Instance.BotMenu.Close();
        }

        private void LoginScript_LoginCompleted(object sender, System.EventArgs e)
        {
            Debug.Log("Login completed", this);
            Debug.Log(ServiceManager.GetService<SlackOidcService>().AccessToken, this);
            // Hide Button
            CloseBotMenu();
            // Show Bot
            Vector3 targetPosition = transform.position - 1f * transform.right;
            targetPosition.y = 0f;
            PhotonNetwork.Instantiate(botPrefab, targetPosition, Quaternion.identity, 0);
            ServiceManager.GetService<SlackOidcService>().LoginCompleted -= LoginScript_LoginCompleted;
            isSubscribedToOidc = false;
            //IUserInfo userInfo = await ServiceManager.GetService<SlackOidcService>().GetUserDataAsync();
        }
    }

}

