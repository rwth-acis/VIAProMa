using ExitGames.Client.Photon;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    /// <summary>
    /// UI controller and actions for the login menu
    /// </summary>
    public class LoginMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private InputField nameInputField;
        [SerializeField] private Interactable doneButton;
        [SerializeField] private InteractableToggleCollection roleToggles;

        public bool WindowEnabled { get; set; }

        public bool WindowOpen => gameObject.activeSelf;

        public event EventHandler WindowClosed;

        private void Awake()
        {
            if (nameInputField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameInputField));
            }
            else
            {
                nameInputField.TextChanged += NameInputChanged;
            }
            if (doneButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(doneButton));
            }
            if (roleToggles == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(roleToggles));
            }
        }

        private void NameInputChanged(object sender, EventArgs e)
        {
            doneButton.IsEnabled = !string.IsNullOrWhiteSpace(nameInputField.Text);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Initialize();
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }

        private void Initialize()
        {
            nameInputField.Text = PhotonNetwork.NickName;
            roleToggles.CurrentIndex = (int)UserManager.Instance.UserRole;
            for (int i = 0; i < roleToggles.ToggleList.Length; i++)
            {
                int dimension = 0;
                if (i == roleToggles.CurrentIndex)
                {
                    dimension = 1;
                }
                roleToggles.ToggleList[i].CurrentDimension = dimension;
            }
        }

        public void SetName()
        {
            if (!string.IsNullOrWhiteSpace(nameInputField.Text))
            {
                PhotonNetwork.NickName = nameInputField.Text;
                RaiseNameChangedEvent();
            }
            Close();
        }

        private void RaiseNameChangedEvent()
        {
            if (PhotonNetwork.IsConnected)
            {
                byte eventCode = 1;
                byte content = 0;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
            }
        }

        public void OnRoleChanged()
        {
            UserManager.Instance.UserRole = (UserRoles)roleToggles.CurrentIndex;
        }
    }
}