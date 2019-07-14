using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the reference to a networked prefab which can be instantiated in a multiplayer scene
/// </summary>
[Serializable]
public class NetworkPrefab
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private string path;

    /// <summary>
    /// The network prefab
    /// </summary>
    /// <value></value>
    public GameObject Prefab { get => prefab; }

    /// <summary>
    /// The relative path to the prefab, starting inside of a resources folder and without the file ending
    /// </summary>
    /// <value></value>
    public string Path
    {
        get
        {
            // first cut away the path leading to the resources
            int startIndex = path.ToLower().IndexOf("resources");
            if (startIndex == -1)
            {
                return "";
            }

            startIndex += ("resources/").Length; // start behind the resources folder

            int endIndex = path.Length - System.IO.Path.GetExtension(path).Length;

            // also cut away the .prefab ending
            return path.Substring(startIndex, endIndex - startIndex);
        }
    }

    /// <summary>
    /// The name of the prefab
    /// This is identical to the file name
    /// </summary>
    /// <value></value>
    public string Name
    {
        get
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }

    /// <summary>
    /// Creates the network prefab object
    /// </summary>
    /// <param name="prefab">The prefab to reference</param>
    /// <param name="path">The full path to the prefab</param>
    public NetworkPrefab(GameObject prefab, string path)
    {
        this.prefab = prefab;
        this.path = path;
    }
}
