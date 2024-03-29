﻿using ExitGames.Client.Photon;
using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars
{
    /// <summary>
    /// Controls the ID card on the avatar and fills its displays with data
    /// </summary>
    public class IdCardController : MonoBehaviour, IOnEventCallback
    {
        [Tooltip("The label which displays the user name")]
        [SerializeField] private TextMeshPro nameLabel;
        [Tooltip("The label which displays the role of the user")]
        [SerializeField] private TextMeshPro roleLabel;
        [Tooltip("The renderer for the profile image")]
        [SerializeField] private Renderer profileImageRenderer;
        [Tooltip("The renderer for the role background")]
        [SerializeField] private Renderer roleBackground;

        [Header("Role Colors")]
        [SerializeField] private Color productOwnerColor;
        [SerializeField] private Color scrumMasterColor;
        [SerializeField] private Color developerColor;
        [SerializeField] private Color StakeHolderColor;

        private PhotonView photonView;
        private UserRoles userRole;


        public UserRoles UserRole
        {
            get => userRole;
            set
            {
                userRole = value;
                roleLabel.text = userRole.GetDescription();
                switch (userRole)
                {
                    case UserRoles.PRODUCT_OWNER:
                        roleBackground.material.color = productOwnerColor;
                        break;
                    case UserRoles.SCRUM_MASTER:
                        roleBackground.material.color = scrumMasterColor;
                        break;
                    case UserRoles.DEVELOPER:
                        roleBackground.material.color = developerColor;
                        break;
                    case UserRoles.STAKEHOLDER:
                        roleBackground.material.color = StakeHolderColor;
                        break;
                }
            }
        }

        /// <summary>
        /// Checks the component's setup and initializes it
        /// </summary>
        private void Awake()
        {
            if (nameLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameLabel));
            }
            if (roleLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(roleLabel));
            }
            if (profileImageRenderer == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(profileImageRenderer));
            }
            if (roleBackground == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(roleBackground));
            }

            photonView = GetComponent<PhotonView>();
        }

        /// <summary>
        /// Initializes the text of the labels
        /// </summary>
        private void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (photonView != null)
                {
                    nameLabel.text = photonView.Owner.NickName;
                }
                else
                {
                    nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
                }
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        [PunRPC]
        private void OnIDCardPropertyUpdated()
        {
            if (photonView != null)
            {
                nameLabel.text = photonView.Owner.NickName;
            }
            else
            {
                nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == 1)
            {
                if (photonView != null)
                {
                    string previousName = nameLabel.text;
                    nameLabel.text = photonView.Owner.NickName;
                    if (previousName != photonView.Owner.NickName)
                    {
                        ChatManager.Instance.AddLocalMessage(previousName + " changed the name to " + photonView.Owner.NickName);
                    }
                }
                else
                {
                    nameLabel.text = PhotonNetwork.LocalPlayer.NickName;
                }
            }
        }
    }
}