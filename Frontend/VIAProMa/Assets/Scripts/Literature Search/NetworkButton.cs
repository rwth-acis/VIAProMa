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

        private void Awake()
        {
            if (paperInScene == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(paperInScene));
            }
        }

        public async void OnButtonClick()
        {
            CitationNetwork network = await CitationNetwork.CreateNetwork(paperInScene.GetComponent<PaperDataDisplay>().Content);
            Debug.Log(network.ToString());
            StartCoroutine(PaperController.Instance.ShowNetwork(network, this.transform));
        }
    }

}
