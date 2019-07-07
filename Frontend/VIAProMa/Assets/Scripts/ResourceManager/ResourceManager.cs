using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private PrefabResourceCollection resourcePrefabCollection;
    [SerializeField] private Texture2D defaultProfileImage;

    protected override void Awake()
    {
        base.Awake();
        resourcePrefabCollection.FindNetworkPrefabsInResources();
        if (defaultProfileImage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(defaultProfileImage));
        }
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

    public Texture2D DefaultProfileImage { get => defaultProfileImage; }
}
