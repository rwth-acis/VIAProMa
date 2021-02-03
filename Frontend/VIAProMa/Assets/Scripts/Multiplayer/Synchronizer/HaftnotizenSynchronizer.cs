using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.Visualizations.Haftnotizen;
using UnityEngine;
using Photon.Pun;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    [RequireComponent(typeof(HaftnotizenVisualController))]
    public class HaftnotizenSynchronizer : TransformSynchronizer
    {
            private HaftnotizenVisualController visualController;

            private string targetText;
            
            private void Awake()
            {
                visualController = GetComponent<HaftnotizenVisualController>();
            }

            public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                base.OnPhotonSerializeView(stream, info);
                if (stream.IsWriting)
                {
                    stream.SendNext(visualController.Text);
                }
                else
                {
                    targetText = (string)stream.ReceiveNext();

                }
            }

            protected override void Update()
            {
                base.Update();
                if (TransformSynchronizationInitialized && photonView.Owner != PhotonNetwork.LocalPlayer)
                {
                    visualController.Text = targetText;
                }
            }
    }
} 