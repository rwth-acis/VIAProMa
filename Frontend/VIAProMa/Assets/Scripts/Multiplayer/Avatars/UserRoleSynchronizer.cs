using i5.VIAProMa.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars
{
    public class UserRoleSynchronizer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private IdCardController idCardController;

        public const string roleKey = "Role";

        private void Awake()
        {
            if (idCardController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(idCardController));
            }
        }

        private void Start()
        {
            // no photonView found => must be a local instance, e.g. in the avatar configurator
            if (photonView == null)
            {
                UserManager.Instance.UserRoleChanged += OnLocalUserRoleChanged;
                idCardController.UserRole = UserManager.Instance.UserRole;
            }
            else
            {
                idCardController.UserRole = (UserRoles)PlayerPropertyUtilities.GetValueOrDefault<byte>(photonView.Owner.CustomProperties, roleKey, 5);
            }
        }

        private void OnLocalUserRoleChanged(object sender, EventArgs e)
        {
            idCardController.UserRole = UserManager.Instance.UserRole;
        }

        public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (photonView == null || !PhotonNetwork.IsConnected)
            {
                idCardController.UserRole = UserManager.Instance.UserRole;
            }
            else if (PhotonNetwork.IsConnected && target == photonView.Owner)
            {
                byte roleIndex = PlayerPropertyUtilities.GetValueOrDefault<byte>(target.CustomProperties, roleKey, 5); // defaults to "developer" if key not found
                UserRoles role = (UserRoles)roleIndex;
                idCardController.UserRole = role;
            }
        }
    }
}