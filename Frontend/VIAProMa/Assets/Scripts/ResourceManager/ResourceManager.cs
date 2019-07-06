using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public PrefabResourceCollection resourcePrefabCollection;

    protected override void Awake()
    {
        base.Awake();
        resourcePrefabCollection.FindNetworkPrefabsInResources();
    }

    public GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        // can only be done with prefabs which are in a Resource folder
        return resourcePrefabCollection.NetworkInstantiate(obj, position, rotation);
    }

    public GameObject NetworkInstantiate(string name, Vector3 position, Quaternion rotation)
    {
        return resourcePrefabCollection.NetworkInstantiate(name, position, rotation);
    }
}
