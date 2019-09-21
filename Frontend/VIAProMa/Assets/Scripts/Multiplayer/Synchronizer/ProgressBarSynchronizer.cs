using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(ProgressBarController))]
public class ProgressBarSynchronizer : TransformSynchronizer
{
    private ProgressBarController progressBarController;

    private void Awake()
    {
        progressBarController = GetComponent<ProgressBarController>();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext(progressBarController.Length);
        }
        else
        {
            progressBarController.Length = (float)stream.ReceiveNext();
        }
    }
}
