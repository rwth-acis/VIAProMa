using UnityEngine;
using Photon.Pun;

/// <summary>
/// The implementation of the IToolAction interface for the remove tool
/// </summary>
public class RemoveActionUndoable : IToolAction
{
    public GameObject target;
    SerializedObject data;

    /// <summary>
    /// Deactivate the target
    /// </summary>
    void IToolAction.DoAction()
    {
        data = target.GetComponentInChildren<Serializer>()?.Serialize();
        PhotonNetwork.Destroy(target);
    }

    /// <summary>
    /// Activate the target
    /// </summary>
    void IToolAction.UndoAction()
    {
        target = ResourceManager.Instance.NetworkInstantiate(data.PrefabName, Vector3.zero, Quaternion.identity);
        target.GetComponentInChildren<Serializer>()?.Deserialize(data);
    }
}
