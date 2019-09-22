using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(KanbanBoardColumnVisualController))]
public class KanbanBoardColumnSynchronizer : TransformSynchronizer
{
    private KanbanBoardColumnVisualController visualController;

    private float targetWidth;
    private float targetHeight;

    private void Awake()
    {
        visualController = GetComponent<KanbanBoardColumnVisualController>();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(visualController.Width);
            stream.SendNext(visualController.Height);
            stream.SendNext((short)visualController.Page);
        }
        else
        {
            targetWidth = (float)stream.ReceiveNext();
            targetHeight = (float)stream.ReceiveNext();
            visualController.Page = (short)stream.ReceiveNext();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (Initialized && photonView.Owner != PhotonNetwork.LocalPlayer)
        {
            visualController.Width = SmoothFloat(visualController.Width, targetWidth, lerpSpeed);
            visualController.Height = SmoothFloat(visualController.Height, targetHeight, lerpSpeed);
        }
    }
}
