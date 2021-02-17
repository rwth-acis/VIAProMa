using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.EventSystems;
using Photon.Pun;

public class RemoveActionUndoable : IToolAction
{
    public GameObject target;

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

    void IToolAction.UndoAction()
    {
        target.SetActive(true);
    }
}
