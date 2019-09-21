using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(KanbanBoardColumnVisualController))]
public class KanbanBoardColumnSynchronizer : TransformSynchronizer
{
    private KanbanBoardColumnVisualController visualController;

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
        }
        else
        {
            visualController.Width = (float)stream.ReceiveNext();
            visualController.Height = (float)stream.ReceiveNext();
        }
    }
}
