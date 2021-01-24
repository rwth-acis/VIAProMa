using i5.VIAProMa.Multiplayer.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(HaftnotizenVisualController))]
public class HaftnotizenSynchronizer : TransformSynchronizer
{
        private HaftnotizenVisualController noteText;

        private string targetText;
        
        private void Awake()
        {
            noteText = GetComponent<HaftnotizenVisualController>();
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            base.OnPhotonSerializeView(stream, info);
            if (stream.IsWriting)
            {
                stream.SendNext(noteText.Text);
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
                noteText.Text = targetText;
            }
        }
}
