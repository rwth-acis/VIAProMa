using HoloToolkit.Unity;
using Photon.Pun;
using System;
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
    [SerializeField] private PhotonView photonView;

    [SerializeField] private PrefabResourceCollection resourcePrefabCollection;
    [SerializeField] private Texture2D defaultProfileImage;

    private short instantiationJobId = 0;
    private Dictionary<short, Action<GameObject>> instanatiationJobCallbacks;

    /// <summary>
    /// Checks the setup and collects the network prefab resources
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (photonView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(photonView));
        }
        if (resourcePrefabCollection == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(resourcePrefabCollection));
        }
        if (defaultProfileImage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(defaultProfileImage));
        }

        resourcePrefabCollection.FindNetworkPrefabsInResources();

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
        return resourcePrefabCollection.NetworkInstantiate(obj, position, rotation, false, data);
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
        return resourcePrefabCollection.NetworkInstantiate(name, position, rotation, false, data);
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
    public void SceneNetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation, Action<GameObject> resultCallback, object[] data = null)
    {
        // only the master client can instantiate new scene objects
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject result = resourcePrefabCollection.NetworkInstantiate(obj, position, rotation, true, data);
            resultCallback?.Invoke(result);
        }
        else
        {
            CallMasterForInstantiation(obj, position, rotation, resultCallback, data);
        }
    }

    private GameObject MasterSceneNetworkInstantiate(string name, Vector3 position, Quaternion rotation, object[] data = null)
    {
        // only the master client can instantiate new scene objects
        if (PhotonNetwork.IsMasterClient)
        {
            return resourcePrefabCollection.NetworkInstantiate(name, position, rotation, true, data);
        }
        else
        {
            Debug.LogError("SceneNetworkInstantiate with name as argument should only be called on Master client");
            return null;
        }
    }

    private async void CallMasterForInstantiation(GameObject obj, Vector3 position, Quaternion rotation, Action<GameObject> resultCallback, object[] data = null)
    {
        // create a remote instantiation job
        short jobId = instantiationJobId;
        instanatiationJobCallbacks.Add(jobId, resultCallback);
        instantiationJobId++;

        short objNameStringId = await NetworkedStringManager.StringToId(obj.name);

        photonView.RPC("RemoteInstantiate", RpcTarget.MasterClient, // consider sending to all clients in order to account for master client switches
            jobId,
            objNameStringId,
            position,
            rotation,
            data
            );
    }

    [PunRPC]
    private async void RemoteInstantiate(
        short jobId,
        short objNameStringId,
        Vector3 position,
        Quaternion rotation,
        object[] data,
        PhotonMessageInfo info)
    {
        // this logic must be executed on the master client
        if (PhotonNetwork.IsMasterClient)
        {
            string objName = await NetworkedStringManager.GetString(objNameStringId);
            GameObject result = MasterSceneNetworkInstantiate(objName, position, rotation, data);
            PhotonView photonViewOnResult = result?.GetComponent<PhotonView>();
            int resultPhotonViewId = 0;
            if (photonViewOnResult != null)
            {
                resultPhotonViewId = photonViewOnResult.ViewID;
            }
            photonView.RPC("RemoteInstantiationFinished", RpcTarget.Others,
                info.Sender.ActorNumber,
                jobId,
                resultPhotonViewId
                );
        }
    }

    [PunRPC]
    private void RemoteInstantiationFinished(
        int querySenderId,
        short jobId,
        int resultPhotonViewId
        )
    {
        // check if the instantiation had been posted by this client
        if (PhotonNetwork.LocalPlayer.ActorNumber == querySenderId)
        {
            // there should be an entry for this job in the dictionary
            if (instanatiationJobCallbacks.ContainsKey(jobId))
            {
                // try to find the given id
                PhotonView res = PhotonView.Find(resultPhotonViewId);
                if (res != null)
                {
                    // call the callback method with the resulting GameObject
                    instanatiationJobCallbacks[jobId].Invoke(res.gameObject);
                }
                else
                {
                    Debug.LogError("RemoteInstantiation could not find the given photon id", gameObject);
                }
            }
            else
            {
                Debug.LogError("RemoteInstantiation received answer for job which was not issued", gameObject);
            }
        }
    }

    /// <summary>
    /// Default profile image which should be shown if no other profile image could be found
    /// </summary>
    /// <value></value>
    public Texture2D DefaultProfileImage { get => defaultProfileImage; }
}
