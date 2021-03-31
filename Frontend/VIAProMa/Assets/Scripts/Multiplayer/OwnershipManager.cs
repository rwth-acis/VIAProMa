using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer
{
    public class OwnershipManager : Singleton<OwnershipManager>, IPunOwnershipCallbacks
    {
        private List<PhotonView> lockedViews;

        protected override void Awake()
        {
            base.Awake();
            lockedViews = new List<PhotonView>();
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void DisableOwnerShipTransfer(PhotonView view)
        {
            // do not allow duplicates and make sure that only the owner can lock objects
            if (!lockedViews.Contains(view) && view.Owner == PhotonNetwork.LocalPlayer)
            {
                lockedViews.Add(view);
            }
        }

        public void EnableOwnerShipTransfer(PhotonView view)
        {
            // make sure that only the owner can unlock objects
            if (view.Owner == PhotonNetwork.LocalPlayer)
            {
                lockedViews.Remove(view);
            }
        }

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + targetView + ".");

            if (!lockedViews.Contains(targetView))
            {
                Debug.Log("Ownership transfer granted for object " + targetView.gameObject.name);
                targetView.TransferOwnership(requestingPlayer);
            }
            else
            {
                Debug.Log("Ownership transfer denied for object " + targetView.gameObject.name);
            }
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
        }
    }
}