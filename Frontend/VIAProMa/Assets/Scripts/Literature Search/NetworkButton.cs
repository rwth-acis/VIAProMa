using i5.VIAProMa.UI.MessageBadge;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class NetworkButton : MonoBehaviour
    {

        [Tooltip("The data display of the base paper for network generation.")]
        [SerializeField] private GameObject paperInScene;
        [Tooltip("The gameobject of the message badge used to indicate loading.")]
        [SerializeField] private GameObject loadingObject;
        [Tooltip("The gameobject of the button used to calculate the next iteration step.")]
        [SerializeField] private GameObject nextIterationButton;

        private MessageBadge _messageBadge;
        private CitationNetwork _network;
        private void Awake()
        {
            if (paperInScene == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(paperInScene));
            }
            if (loadingObject == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loadingObject));
            }
            else
            {
                _messageBadge = loadingObject.GetComponent<MessageBadge>();
            }
            if (nextIterationButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(nextIterationButton));
            }
        }

        /// <summary>
        /// Create the citation network to the given paper.
        /// </summary>
        public async void OnButtonClick()
        {
            StartLoading();

            // Calculate citation network
            _network = await CitationNetwork.CreateNetwork(paperInScene.GetComponent<PaperDataDisplay>().Content);
            EndLoading();

            // Visualize citation network
            StartCoroutine(PaperController.Instance.ShowNetwork(_network, this.transform));
        }

        private void StartLoading()
        { 
            nextIterationButton.GetComponent<Interactable>().IsEnabled = false;
            _messageBadge.gameObject.SetActive(true);
            _messageBadge.ShowProcessing();
        }

        private void EndLoading()
        {
            _messageBadge.DoneProcessing();

            nextIterationButton.GetComponent<Interactable>().IsEnabled = true;
        }

        public async void NextIterationStep()
        {
            StartLoading();

            _network = await _network.CalculateNextIteration();
            EndLoading();
            
            StartCoroutine(PaperController.Instance.ShowNetwork(_network, this.transform));
        }
    }

}
