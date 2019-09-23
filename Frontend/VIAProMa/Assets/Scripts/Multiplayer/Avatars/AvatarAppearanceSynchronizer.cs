using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Synchronizes the appearance of the avatar among clients
/// </summary>
[RequireComponent(typeof(AvatarConfigurationController))]
public class AvatarAppearanceSynchronizer : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The suffix which marks the model index key
    /// </summary>
    public const string modelKeySuffix = "Model";
    /// <summary>
    /// The suffix which marks the material index key
    /// </summary>
    public const string materialKeySuffix = "Material";
    /// <summary>
    /// The suffix which marks the color index key
    /// </summary>
    public const string colorKeySuffix = "Color";

    private AvatarConfigurationController configurationController;

    /// <summary>
    /// Initializes the component
    /// </summary>
    private void Awake()
    {
        configurationController = GetComponent<AvatarConfigurationController>();
    }

    /// <summary>
    /// Applies the avatar configuration
    /// This is necessary since the other persion who is represented by the avtar might already have changed it
    /// </summary>
    private void Start()
    {
        // apply avatar configuration when the avatar is created
        ApplyCustomProperties();
    }

    /// <summary>
    /// Called by Photon if the properties on a player are updated
    /// Updates the configuration if the target player is the owner of this avatar
    /// </summary>
    /// <param name="target">The player whose properties were changed</param>
    /// <param name="changedProps">Contains the keys and values of the changed properties</param>
    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (target.UserId == photonView.Owner.UserId)
        {
            ApplyCustomProperties();
        }
    }

    /// <summary>
    /// Goes over all avatar part configuration categories and applies the corresponding property indices
    /// </summary>
    private void ApplyCustomProperties()
    {
        // only do this if the client is connected; do not work on the own avatar which is not visible
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        foreach(AvatarPartControllerCategory controllerCategory in configurationController.AvatarPartControllers)
        {
            ApplyCustomProperty(controllerCategory.ConfigurationController, controllerCategory.Name);
        }
    }

    /// <summary>
    /// Applies the avatar part configuration for a specific part controller
    /// </summary>
    /// <param name="partController">The part controller to which the properties should be applied</param>
    /// <param name="partName">The name of the part (used in the hastable keys)</param>
    private void ApplyCustomProperty(AvatarPartConfigurationController partController, string partName)
    {
        partController.ModelIndex = GetValueOrDefault<byte>(photonView.Owner.CustomProperties, partName + modelKeySuffix, 0);
        partController.MaterialIndex = GetValueOrDefault<byte>(photonView.Owner.CustomProperties, partName + materialKeySuffix, 0);
        partController.ColorIndex = GetValueOrDefault<byte>(photonView.Owner.CustomProperties, partName + colorKeySuffix, 0);

        partController.ApplyConfiguration();
    }

    /// <summary>
    /// Tries to retrieve a value based on the given key from the hastable
    /// If the key is not found, a default value is returned
    /// </summary>
    /// <typeparam name="T">The type of the value in the hashtable</typeparam>
    /// <param name="hastable">The hastable to query</param>
    /// <param name="key">The key for which the value should be retrieved</param>
    /// <param name="defaultValue">A default value in case that the key does not exist</param>
    /// <returns>The value stored under the key entry or the default value if the key does not exist</returns>
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

    /// <summary>
    /// Sets an index in the player's properties to the given value
    /// </summary>
    /// <param name="name">The key name</param>
    /// <param name="value">The value which should be added to the properties</param>
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
