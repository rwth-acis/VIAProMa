using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipRequester : MonoBehaviourPun
{
    public void OnManipulationStarted()
    {
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            Debug.Log("Locking object", gameObject);
            OwnershipManager.Instance.DisableOwnerShipTransfer(photonView);
        }
        else
        {
            Debug.Log("Requesting ownership", gameObject);
            photonView.RequestOwnership();
        }
    }

    public void OnManipulationEnded()
    {
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
        {
            OwnershipManager.Instance.EnableOwnerShipTransfer(photonView);
        }
    }
}
