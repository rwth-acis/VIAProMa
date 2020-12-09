using HoloToolkit.Unity;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    private UserRoles role = UserRoles.STUDENT;
    private string defaultName;

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
