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
                Debug.Log("-----------------------------------Awoke Sync");
            }

            public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                base.OnPhotonSerializeView(stream, info);
            Debug.Log("-----------------------------------Stream start");
                if (stream.IsWriting)
                {
                    Debug.Log("-----------------------------------Stream is writing");
                    stream.SendNext(visualController.Text);
                }
                else
                {
                    Debug.Log("-----------------------------------Stream is recieving next");
                    targetText = (string)stream.ReceiveNext();

                }
            }

            protected override void Update()
            {
                //Debug.Log("-----------------------------------Trying to update");
                base.Update();
                if (TransformSynchronizationInitialized && photonView.Owner != PhotonNetwork.LocalPlayer)
                {
                    Debug.Log("-----------------------------------Final part");
                    visualController.Text = targetText;
                }
            }
    }
}