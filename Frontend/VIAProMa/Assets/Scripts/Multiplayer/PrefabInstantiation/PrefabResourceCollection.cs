using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkPrefabs", menuName = "Scriptable Objects/Networked Prefab Collection")]
public class PrefabResourceCollection : SingletonScriptableObject<PrefabResourceCollection>
{
    [SerializeField] private List<NetworkPrefab> networkPrefabs;

    public static GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        foreach(NetworkPrefab networkPrefab in Instance.networkPrefabs)
        {
            if (networkPrefab.Prefab == obj)
            {
                return PhotonNetwork.Instantiate(networkPrefab.Path, position, rotation);
            }
        }

        Debug.LogWarning("Prefab was not found. Maybe it is not in a resources folder or it does not have a PhotonView?");
        return null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void FindNetworkPrefabsInResources()
    {
#if UNITY_EDITOR
        if (Instance != null)
        {
            Debug.Log("Updating Networked Prefabs...");
            Instance.networkPrefabs.Clear();
            GameObject[] prefabs = Resources.LoadAll<GameObject>("");
            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i].GetComponent<PhotonView>() != null) // only look for networked gameobjects
                {
                    string path = AssetDatabase.GetAssetPath(prefabs[i]);
                    Instance.networkPrefabs.Add(new NetworkPrefab(prefabs[i], path));
                }
            }
        }
#endif
    }
}
