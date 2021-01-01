using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.Visualizations.BuildingProgressBar;
using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
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
}