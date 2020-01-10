﻿using HoloToolkit.Unity;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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