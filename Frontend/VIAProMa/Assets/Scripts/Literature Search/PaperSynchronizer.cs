using i5.VIAProMa.Multiplayer.Common;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    [RequireComponent(typeof(PaperDataDisplay))]
    public class PaperSynchronizer : TransformSynchronizer
    {
        private PaperDataDisplay paperDataDisplay;
        /// <summary>
        /// Instantiates the privates variable when awoken.
        /// </summary>
        private void Awake()
        {
            paperDataDisplay = GetComponent<PaperDataDisplay>();
        }

        /// <summary>
        /// Synchronizes the paper when started.
        /// </summary>
        private async void Start()
        {
            // in case that the card was already setup, e.g. in the local instance => do not setup again
            if (paperDataDisplay.Content != null)
            {
                Debug.Log("Card was already setup");
                return;
            }

            if (photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                Debug.Log("Card created by local player; will not set up");
                return;
            }

            string doi;

            if (photonView.InstantiationData.Length == 1) 
            { 
                doi = (string)photonView.InstantiationData[0];
                CrossRefPaper result = await Communicator.GetPaper(doi);    // request data for specific doi from CrossRef
                if (result != null)
                {
                    paperDataDisplay.Setup(result.ToPaper());
                }
                else
                {
                    Debug.LogError("Paper synchronizer could not fetch paper with doi " + doi);
                }
            }
            else
            {
                Debug.Log("Unexpected number of instantiation data on issue");
                return;
            }
        }
    }

}
