using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer;
using i5.VIAProMa.Multiplayer.Avatars;
using Photon.Pun;
using System;

namespace i5.VIAProMa
{
    public class UserManager : Singleton<UserManager>
    {
        private UserRoles role = UserRoles.DEVELOPER;

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

    public string DefaultName
    {
        get
        {
            if (string.IsNullOrEmpty(defaultName))
            {
                //defaultName = "Guest" + UnityEngine.Random.Range(0, 1000);
                defaultName = "Chen-Ching Teng";
            }
            return defaultName;
        }
    }
}
