using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Collection of the networked prefabs which are available in a resource folder and have an attached phton view
/// </summary>
[CreateAssetMenu(fileName = "NetworkPrefabs", menuName = "Scriptable Objects/Networked Prefab Collection")]
public class PrefabResourceCollection : ScriptableObject
{
    [SerializeField] private List<NetworkPrefab> networkPrefabs;

    public List<NetworkPrefab> NetworkPrefabs { get => networkPrefabs; }

    /// <summary>
    /// Instantiates the given GameObject in the network
    /// The GameObject must have a prefab in the resources folder with attached photon view
    /// </summary>
    /// <param name="obj">The GameObject to instantiate</param>
    /// <param name="position">The position where the GameObject should be instantiated</param>
    /// <param name="rotation">The rotation with which the GameObject should be instantiated</param>
    /// <returns>The created GameObject in the scene</returns>
    public GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        foreach (NetworkPrefab networkPrefab in networkPrefabs)
        {
            if (networkPrefab.Prefab == obj)
            {
                return PhotonNetwork.Instantiate(networkPrefab.Path, position, rotation);
            }
        }

        Debug.LogWarning("Prefab was not found. Maybe it is not in a resources folder or it does not have a PhotonView?");
        return null;
    }

    /// <summary>
    /// Instantiates the prefab with the given name in the network
    /// </summary>
    /// <param name="name">The name of the prefab</param>
    /// <param name="position">The position where the GameObject should be instantiated</param>
    /// <param name="rotation">The rotation with which the GameObject should be instantiated</param>
    /// <returns>The created GameObject in the scene</returns>
    public GameObject NetworkInstantiate(string name, Vector3 position, Quaternion rotation)
    {
        foreach (NetworkPrefab networkPrefab in networkPrefabs)
        {
            if (networkPrefab.Name == name)
            {
                return PhotonNetwork.Instantiate(networkPrefab.Path, position, rotation);
            }
        }

        Debug.LogWarning("Prefab not found. Maybe it is not in a resources folder or it does not have a PhotonView?");
        return null;
    }

    /// <summary>
    /// Searches the resources for prefabs which have a photon view
    /// The found prefabs which match this criterion are collected and stored in this scriptable object
    /// </summary>
    public void FindNetworkPrefabsInResources()
    {
#if UNITY_EDITOR
        Debug.Log("Updating Networked Prefabs...");
        networkPrefabs.Clear();
        // search all prefabs in resources folders
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");

        List<string> names = new List<string>();

        // go over all found prefabs
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].GetComponent<PhotonView>() != null) // only look for networked gameobjects
            {
                string path = AssetDatabase.GetAssetPath(prefabs[i]);
                NetworkPrefab networkPrefab = new NetworkPrefab(prefabs[i], path);
                if (names.Contains(networkPrefab.Name))
                {
                    Debug.LogWarning("There are multiple prefabs with the name " + networkPrefab.Name + ". This can lead to strange behaviour when instantiating by name.");
                }
                names.Add(networkPrefab.Name);
                networkPrefabs.Add(networkPrefab);
            }
        }

        EditorUtility.SetDirty(this);
#endif
    }
}
