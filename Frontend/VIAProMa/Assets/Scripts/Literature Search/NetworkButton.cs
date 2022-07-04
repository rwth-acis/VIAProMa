using i5.VIAProMa.UI.MessageBadge;
using i5.VIAProMa.Utilities;
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

        private MessageBadge _messageBadge;
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
        }

        /// <summary>
        /// Create the citation network to the given paper.
        /// </summary>
        public async void OnButtonClick()
        {
            StartLoading();

            // Calculate citation network
            CitationNetwork network = await CitationNetwork.CreateNetwork(paperInScene.GetComponent<PaperDataDisplay>().Content);

            EndLoading();

            // Visualize citation network
            StartCoroutine(PaperController.Instance.ShowNetwork(network, this.transform));
        }

        private void StartLoading()
        {
            _messageBadge.gameObject.SetActive(true);
            _messageBadge.ShowProcessing();
        }

        private void EndLoading()
        {
            _messageBadge.DoneProcessing();
        }
    }

}
