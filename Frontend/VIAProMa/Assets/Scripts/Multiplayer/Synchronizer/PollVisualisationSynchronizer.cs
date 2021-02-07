using i5.VIAProMa.Multiplayer.Common;
using i5.VIAProMa.WebConnection;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using i5.VIAProMa.Visualizations.Poll;
using i5.VIAProMa.Multiplayer.Poll;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    /// <summary>
    /// Synchronizes the poll visualization
    /// </summary>
    [RequireComponent(typeof(PollBarVisualization))]
    public class PollVisualisationSynchronizer : TransformSynchronizer
    {
        private PollBarVisualization pollViz;

        private void Awake()
        {
            pollViz = GetComponent<PollBarVisualization>();
            pollViz.PollVizUpdated += SendUpdatePollRequest;
        }

        private void OnDestroy()
        {
            pollViz.PollVizUpdated -= SendUpdatePollRequest;
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            base.OnPhotonSerializeView(stream, info);
            if (stream.IsWriting)
            {
                stream.SendNext(pollViz.pollIndex);
            }
            else
            {
                pollViz.UpdatePoll((int)stream.ReceiveNext());
            }
        }

        private void SendUpdatePollRequest(object sender, EventArgs args)
        {
            photonView.RPC("UpdatePoll", RpcTarget.Others, pollViz.pollIndex);
        }

        [PunRPC]
        private async void UpdatePoll(int pollIndex)
        {
            pollViz.SetupPoll(pollIndex);
        }
    }
}