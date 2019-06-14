using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NetworkPrefab
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private string path;

    [SerializeField] private string debugPath;

    public GameObject Prefab { get => prefab; }
    public string Path
    {
        get
        {
            // first cut away the path leading to the Resources
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

    public NetworkPrefab(GameObject prefab, string path)
    {
        this.prefab = prefab;
        this.path = path;
        debugPath = Path;
    }
}
