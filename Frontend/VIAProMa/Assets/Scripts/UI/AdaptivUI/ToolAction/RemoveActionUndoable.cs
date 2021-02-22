using UnityEngine;
using Photon.Pun;

/// <summary>
/// The implementation of the IToolAction interface for the remove tool
/// </summary>
public class RemoveActionUndoable : IToolAction
{
    public GameObject target;

    /// <summary>
    /// Deactivate the target
    /// </summary>
    void IToolAction.DoAction()
    {
        if (target.GetComponentInChildren<PhotonView>() != null)
        {
            //PhotonNetwork.Destroy(target);
            target.SetActive(false);
        }
        else
        {
            //Destroy(target);
            target.SetActive(false);
        }
    }

    /// <summary>
    /// Activate the target
    /// </summary>
    void IToolAction.UndoAction()
    {
        target.SetActive(true);
    }
}
