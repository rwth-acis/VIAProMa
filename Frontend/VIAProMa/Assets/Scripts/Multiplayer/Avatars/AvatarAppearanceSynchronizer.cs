using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarConfigurationController))]
public class AvatarAppearanceSynchronizer : MonoBehaviourPunCallbacks
{
    AvatarConfigurationController configurationController;

    private void Awake()
    {
        configurationController = GetComponent<AvatarConfigurationController>();
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Properties changed for " + target.NickName);
        if (target == photonView.Owner)
        {
            ApplyCustomProperties();
        }
    }

    private void ApplyCustomProperties()
    {
        if (!PhotonNetwork.IsConnected || photonView.IsMine)
        {
            return;
        }

        ApplyCustomProperty(configurationController.HairCategory.ConfigurationController, configurationController.HairCategory.Name);
        ApplyCustomProperty(configurationController.GlassesCategory.ConfigurationController, configurationController.GlassesCategory.Name);
        ApplyCustomProperty(configurationController.ClothesCategory.ConfigurationController, configurationController.ClothesCategory.Name);
    }

    private void ApplyCustomProperty(AvatarPartConfigurationController partController, string partName)
    {
        partController.ModelIndex = (byte)photonView.Owner.CustomProperties[partName + "Model"];
        partController.MaterialIndex = (byte)photonView.Owner.CustomProperties[partName + "Material"];
        partController.ColorIndex = (byte)photonView.Owner.CustomProperties[partName + "Color"];

        partController.ApplyConfiguration();
    }
}
