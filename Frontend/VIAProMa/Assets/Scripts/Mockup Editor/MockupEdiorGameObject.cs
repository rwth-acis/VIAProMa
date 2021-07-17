using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// component which is attacked to every mockup item instance
/// </summary>
public class MockupEdiorGameObject : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public string category;
    public int index;

    /// <summary>
    /// called when the object is spawned on the network, called by everyone
    /// </summary>
    /// <param name="info"></param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        category = (string)instantiationData[0];
        index = (int)instantiationData[1];
        SpawnChildObject();
    }

    /// <summary>
    /// sets the data of the item so which item it represents
    /// </summary>
    /// <param name="category">the path of the category list inside a resource folder</param>
    /// <param name="index">the index of the represented GO inside the list</param>
    public void SetData(string category, int index)
    {
        this.category = category;
        this.index = index;
    }

    /// <summary>
    /// spawns the childobject which contains the skin
    /// </summary>
    public void SpawnChildObject()
    {
        if(transform.childCount <= 2) //if less than 3 childs attached, there is no skin currently attached
        {
            MockupEditorList list = Resources.Load<MockupEditorList>(category);
            Instantiate(list.items[index].Prefab, transform);            
        }
    }
}
