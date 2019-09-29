using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPropertyUtilities
{
    /// <summary>
    /// Tries to retrieve a value based on the given key from the hastable
    /// If the key is not found, a default value is returned
    /// </summary>
    /// <typeparam name="T">The type of the value in the hashtable</typeparam>
    /// <param name="hastable">The hastable to query</param>
    /// <param name="key">The key for which the value should be retrieved</param>
    /// <param name="defaultValue">A default value in case that the key does not exist</param>
    /// <returns>The value stored under the key entry or the default value if the key does not exist</returns>
    public static T GetValueOrDefault<T>(ExitGames.Client.Photon.Hashtable hastable, string key, T defaultValue)
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
