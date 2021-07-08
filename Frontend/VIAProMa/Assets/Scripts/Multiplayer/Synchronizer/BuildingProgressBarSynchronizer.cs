using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.Visualizations.BuildingProgressBar;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(BuildingProgressBarVisuals))]
public class BuildingProgressBarSynchronizer : TransformSynchronizer
{
    private BuildingProgressBarVisuals barVisuals;

    private void Awake()
    {
        barVisuals = GetComponent<BuildingProgressBarVisuals>();
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
        if (stream.IsWriting)
        {
            stream.SendNext((short)barVisuals.BuildingModelIndex);
        }
        else
        {
            barVisuals.BuildingModelIndex = (short)stream.ReceiveNext();
        }
    }
}
