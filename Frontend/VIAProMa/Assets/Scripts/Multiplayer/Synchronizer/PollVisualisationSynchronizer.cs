using i5.VIAProMa.Multiplayer.Common;
using Photon.Pun;
using System;
using UnityEngine;
using i5.VIAProMa.Visualizations.Poll;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    /// <summary>
    /// Synchronizes the PollBarVisualization
    /// </summary>
    [RequireComponent(typeof(PollBarVisualization))]
    public class PollVisualisationSynchronizer : TransformSynchronizer
    {
        private PollBarVisualization pollViz;

        private void Awake()
        {
            pollViz = GetComponent<PollBarVisualization>();
            pollViz.PollVizUpdatedForced += SendUpdatePollRequest;
        }

        private void OnDestroy()
        {
            pollViz.PollVizUpdatedForced -= SendUpdatePollRequest;
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            base.OnPhotonSerializeView(stream, info);
            if (stream.IsWriting)
            {
                stream.SendNext(pollViz.pollID);
            }
            else
            {
                pollViz.UpdatePoll((int)stream.ReceiveNext());
            }
        }

        private void SendUpdatePollRequest(object sender, EventArgs args)
        { // Local visualization has been forcefully updated (keeping ID same, but content changed)
            photonView.RPC("UpdatePoll", RpcTarget.Others, pollViz.pollID);
        }

        [PunRPC]
        private void UpdatePoll(int pollID)
        { // Force update poll visualization (from synchronized poll database), potentially this has not been updated before
            pollViz.SetupPoll(pollID);
        }
    }
}