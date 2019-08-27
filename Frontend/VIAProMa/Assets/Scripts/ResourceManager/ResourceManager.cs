using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager which administers references to resources and prefabs
/// This way, the reference can be set on one central object and all other components in the scene can access it from here
/// </summary>
/// <typeparam name="ResourceManager"></typeparam>
public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] private PrefabResourceCollection resourcePrefabCollection;
    [SerializeField] private Texture2D defaultProfileImage;

    /// <summary>
    /// Checks the setup and collects the network prefab resources
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        resourcePrefabCollection.FindNetworkPrefabsInResources();
        if (resourcePrefabCollection == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(resourcePrefabCollection));
        }
        if (defaultProfileImage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(defaultProfileImage));
        }

        Debug.Log("Networked Prefabs: " + resourcePrefabCollection.NetworkPrefabs.Count);
    }

    /// <summary>
    /// Instantiates the given GameObject in the scene
    /// The method does not directly use the referenced GameObject but searches for the equivalent prefab in the resources
    /// If the prefab does not exist in the resources, null is returned
    /// </summary>
    /// <param name="obj">The GameObject to instantiate</param>
    /// <param name="position">The position where the object should be instantiated</param>
    /// <param name="rotation">The rotatoin with which the object should be instantiated</param>
    /// <param name="data">Instantiation data which can be added to the object</param>
    /// <returns>The created instance in the scene</returns>
    public GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation, object[] data = null)
    {
        // can only be done with prefabs which are in a Resource folder
        return resourcePrefabCollection.NetworkInstantiate(obj, position, rotation, data);
    }

    /// <summary>
    /// Instantiates the GameObject with the given name in the scene
    /// The GameObject must exist in the resources and have a photon view
    /// </summary>
    /// <param name="name">The name of the GameObjects</param>
    /// <param name="position">The position where the object should be instantiated</param>
    /// <param name="rotation">The rotatoin with which the object should be instantiated</param>
    /// <param name="data">Instantiation data which can be added to the object</param>
    /// <returns>The created instance in the scene</returns>
    public GameObject NetworkInstantiate(string name, Vector3 position, Quaternion rotation, object[] data = null)
    {
        return resourcePrefabCollection.NetworkInstantiate(name, position, rotation, data);
    }

    /// <summary>
    /// Instantiates the given GameObject as a scene object whose lifetime is coupled with the room and scene
    /// This way, the GameObject will still exist if the creator left the room
    /// The GameObject must exist in the resources and have a photon view
    /// </summary>
    /// <param name="obj">The GameObject to instantiate</param>
    /// <param name="position">The position where the object should be instantiated</param>
    /// <param name="rotation">The rotatoin with which the object should be instantiated</param>
    /// <param name="data">Instantiation data which can be added to the object</param>
    /// <returns></returns>
    public GameObject SceneNetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation, object[] data = null)
    {
        return resourcePrefabCollection.SceneNetworkInstantiate(obj, position, rotation, data);
    }

    /// <summary>
    /// Default profile image which should be shown if no other profile image could be found
    /// </summary>
    /// <value></value>
    public Texture2D DefaultProfileImage { get => defaultProfileImage; }
}
