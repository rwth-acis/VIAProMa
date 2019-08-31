using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnershipManager : Singleton<OwnershipManager>
{
    private List<PhotonView> lockedViews;

    protected override void Awake()
    {
        base.Awake();
        lockedViews = new List<PhotonView>();
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

    public void OnOwnershipRequest(object[] viewAndPlayer)
    {
        PhotonView view = (PhotonView)viewAndPlayer[0];
        Player requestingPlayer = (Player)viewAndPlayer[1];

        Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + view + ".");

        if (!lockedViews.Contains(view))
        {
            view.TransferOwnership(requestingPlayer);
        }
    }
}
