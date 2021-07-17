using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.Visualizations.Minimap;
using Photon.Pun;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    class MinimapSynchronizer : TransformSynchronizer
    {
        private MinimapController minimapController;

        private float targetSize;

        private void Awake()
        {
            minimapController = GetComponent<MinimapController>();
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            base.OnPhotonSerializeView(stream, info);
            if (stream.IsWriting)
            {
                stream.SendNext(minimapController.Width);
            }
            else
            {
                targetSize = (float) stream.ReceiveNext();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (TransformSynchronizationInitialized && photonView.Owner != PhotonNetwork.LocalPlayer)
            {
                minimapController.Width = SmoothFloat(minimapController.Width, targetSize, lerpSpeed);
                minimapController.Height = SmoothFloat(minimapController.Width, targetSize, lerpSpeed);
            }
        }
    }
}
