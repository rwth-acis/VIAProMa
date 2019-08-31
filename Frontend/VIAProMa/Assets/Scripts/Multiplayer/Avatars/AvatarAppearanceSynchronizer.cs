﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarConfigurationController))]
public class AvatarAppearanceSynchronizer : MonoBehaviourPunCallbacks
{
    public const string modelKeySuffix = "Model";
    public const string materialKeySuffix = "Material";
    public const string colorKeySuffix = "Color";

    private AvatarConfigurationController configurationController;

    private void Awake()
    {
        configurationController = GetComponent<AvatarConfigurationController>();
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (target.UserId == photonView.Owner.UserId)
        {
            ApplyCustomProperties();
        }
    }

    private void ApplyCustomProperties()
    {
        // only do this if the client is connected; do not work on the own avatar which is not visible
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        ApplyCustomProperty(configurationController.HairCategory.ConfigurationController, configurationController.HairCategory.Name);
        ApplyCustomProperty(configurationController.GlassesCategory.ConfigurationController, configurationController.GlassesCategory.Name);
        ApplyCustomProperty(configurationController.ClothesCategory.ConfigurationController, configurationController.ClothesCategory.Name);
    }

    private void ApplyCustomProperty(AvatarPartConfigurationController partController, string partName)
    {
        partController.ModelIndex = GetValueOrDefault<byte>(photonView.Owner.CustomProperties, partName + modelKeySuffix, 0);
        partController.MaterialIndex = GetValueOrDefault<byte>(photonView.Owner.CustomProperties, partName + materialKeySuffix, 0);
        partController.ColorIndex = GetValueOrDefault<byte>(photonView.Owner.CustomProperties, partName + colorKeySuffix, 0);

        partController.ApplyConfiguration();
    }

    private static T GetValueOrDefault<T>(ExitGames.Client.Photon.Hashtable hastable, string key, T defaultValue)
    {
        if (hastable.ContainsKey(key))
        {
            return (T)hastable[key];
        }
        else
        {
            return defaultValue;
        }
    }

    public static void SetProperty(string name, byte value)
    {
        // can only change a property if connected
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        customProperties.Add(name, value);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }
}