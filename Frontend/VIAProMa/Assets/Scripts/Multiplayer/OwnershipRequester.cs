using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer
{
    public class OwnershipRequester : MonoBehaviourPun
    {
        public void EnsureOwnership()
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

        public void ReleaseOwnershipLock()
        {
            if (photonView.Owner == PhotonNetwork.LocalPlayer)
            {
                Debug.Log("Unlocking object", gameObject);
                OwnershipManager.Instance.EnableOwnerShipTransfer(photonView);
            }
        }
    }
}