using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkPrefabs", menuName = "Scriptable Objects/Networked Prefab Collection")]
public class PrefabResourceCollection : ScriptableObject
{
    [SerializeField] private List<NetworkPrefab> networkPrefabs;

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public void FindNetworkPrefabsInResources()
    {
#if UNITY_EDITOR
        Debug.Log("Updating Networked Prefabs...");
        networkPrefabs.Clear();
        GameObject[] prefabs = Resources.LoadAll<GameObject>("");

        List<string> names = new List<string>();

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
#endif
    }
}
