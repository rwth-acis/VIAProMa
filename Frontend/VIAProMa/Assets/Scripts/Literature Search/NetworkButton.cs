using i5.VIAProMa.UI.MessageBadge;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    /// <summary>
    /// Class for the network button.
    /// </summary>
    public class NetworkButton : MonoBehaviour
    {

        [Tooltip("The data display of the base paper for network generation.")]
        [SerializeField] private GameObject paperInScene;
        [Tooltip("The gameobject of the message badge used to indicate loading.")]
        [SerializeField] private GameObject loadingObject;
        [Tooltip("The gameobject of the button used to calculate the next iteration step.")]
        [SerializeField] private GameObject nextIterationButton;

        /// <summary>
        /// MessageBadge used to display the loading animation.
        /// </summary>
        private MessageBadge _messageBadge;
        /// <summary>
        /// The citation network calculated and iterated by the class.
        /// </summary>
        private CitationNetwork _network;
        /// <summary>
        /// Network button belonging to this object.
        /// </summary>
        private Interactable _button;

        /// <summary>
        /// Initializes the window and makes sure all UI Elements are referenced and the window is set up correctly.
        /// </summary>
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
            if(GetComponent<Interactable>() != null)
            {
                _button = GetComponent<Interactable>();
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

        /// <summary>
        /// Starts the loading animation
        /// </summary>
        private void StartLoading()
        { 
            nextIterationButton.GetComponent<Interactable>().IsEnabled = false;
            _messageBadge.gameObject.SetActive(true);
            _messageBadge.ShowProcessing();
            if(_button != null)
            {
                _button.IsEnabled = false;
            }
        }

        /// <summary>
        /// Ends the loading animation.
        /// </summary>
        private void EndLoading()
        {
            _messageBadge.DoneProcessing();

            if(_button != null)
            {
                _button.IsEnabled = true;
            }
            nextIterationButton.GetComponent<Interactable>().IsEnabled = true;
        }

        /// <summary>
        /// Calculates and visualizes the next iteration step of the citation network.
        /// </summary>
        public async void NextIterationStep()
        {
            StartLoading();

            _network = await _network.CalculateNextIteration();
            EndLoading();
            
            StartCoroutine(PaperController.Instance.ShowNetwork(_network, this.transform));
        }
    }

}
