using i5.VIAProMa.Visualizations.Competence;
using i5.VIAProMa.WebConnection;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Synchronizer
{
    [RequireComponent(typeof(CompetenceDisplayVisualController))]
    public class CompetenceDisplaySynchronizer : MonoBehaviourPunCallbacks
    {
        private CompetenceDisplay competenceDisplay;

        private int remoteSynchronizations = 0;
        private bool initialized = false;

        private bool RemoteSynchronizationInProgress { get => remoteSynchronizations > 0; }

        private void Awake()
        {
            competenceDisplay = GetComponent<CompetenceDisplay>();
        }

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                initialized = true;
                SendInitializationData();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            competenceDisplay.FilterChanged += OnFilterChanged;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            competenceDisplay.FilterChanged -= OnFilterChanged;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient && newPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                SendInitializationData();
            }
        }

        private async void SendInitializationData()
        {
            if (!PhotonNetwork.IsConnected)
            {
                return;
            }

            string combinedFilter = CombineFilter(competenceDisplay.FilterWords);
            short filterId = await NetworkedStringManager.StringToId(combinedFilter);

            // change this to an initialization method if more properties need to be synchronized
            photonView.RPC("SetFilter", RpcTarget.Others, filterId);

        }

        private async void OnFilterChanged(object sender, EventArgs e)
        {
            if (RemoteSynchronizationInProgress || !initialized || !PhotonNetwork.IsConnected)
            {
                return;
            }

            string combinedFilter = CombineFilter(competenceDisplay.FilterWords);

            short filterId = await NetworkedStringManager.StringToId(combinedFilter);
            photonView.RPC("SetFilter", RpcTarget.Others, filterId);
        }

        private string CombineFilter(string[] filterWorlds)
        {
            if (filterWorlds == null)
            {
                return "";
            }

            string combinedFilter = "";
            for (int i = 0; i < filterWorlds.Length; i++)
            {
                combinedFilter += competenceDisplay.FilterWords[i];
                if (i < filterWorlds.Length - 1)
                {
                    combinedFilter += ";";
                }
            }
            return combinedFilter;
        }

        [PunRPC]
        private async void SetFilter(short filterId)
        {
            Debug.Log("RPC: Setting filter", gameObject);
            string filter = await NetworkedStringManager.GetString(filterId);
            remoteSynchronizations++;
            competenceDisplay.FilterWords = filter.Split(';');
            remoteSynchronizations--;
        }
    }
}