using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipRequester : MonoBehaviourPun
{
    public void OnManipulationStarted()
    {
        // since we are dealing with scene objects, there is no owner in the beginning
        if (photonView.Owner != null)
        {
            Debug.Log(photonView.Owner + " vs. " + PhotonNetwork.LocalPlayer);
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
        else
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
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
