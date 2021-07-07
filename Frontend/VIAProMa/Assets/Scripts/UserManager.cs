using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.Multiplayer.Avatars;
using Photon.Pun;
using System;

namespace i5.VIAProMa
{
    public class UserManager : Singleton<UserManager>
    {
        private UserRoles role = UserRoles.STUDENT;

        public event EventHandler UserRoleChanged;

        public UserRoles UserRole
        {
            get => role;
            set
            {
                role = value;
                if (PhotonNetwork.IsConnected)
                {
                    PlayerPropertyUtilities.SetProperty(UserRoleSynchronizer.roleKey, (byte)role);
                }
                UserRoleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}