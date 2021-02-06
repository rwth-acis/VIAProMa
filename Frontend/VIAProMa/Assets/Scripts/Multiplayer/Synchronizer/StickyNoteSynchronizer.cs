using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.Visualizations.StickyNote;
using UnityEngine;
using Photon.Pun;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    [RequireComponent(typeof(StickyNoteVisualController))]
    public class StickyNoteSynchronizer : TransformSynchronizer
    {
            private StickyNoteVisualController visualController;

            private string targetText;
            private string targetColorName;
            
            private void Awake()
            {
                visualController = GetComponent<StickyNoteVisualController>();
            }

            public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                base.OnPhotonSerializeView(stream, info);
                if (stream.IsWriting)
                {
                    stream.SendNext(visualController.Text);
                    stream.SendNext(visualController.ColorTag);
                }
                else
                {
                    targetText = (string)stream.ReceiveNext();
                    targetColorName = (string)stream.ReceiveNext();

                }
            }

            protected override void Update()
            {
                base.Update();
                if (TransformSynchronizationInitialized && photonView.Owner != PhotonNetwork.LocalPlayer)
                {
                    visualController.Text = targetText;
                    visualController.ColorTag = targetColorName;
                }
            }
    }
} 