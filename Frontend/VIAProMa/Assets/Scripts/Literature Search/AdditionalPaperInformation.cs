using i5.VIAProMa.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class AdditionalPaperInformation : MonoBehaviour
    {
        [SerializeField] private GameObject paperInScene;
        [SerializeField] private GameObject infoButton;

        private void Awake()
        {

            if (paperInScene == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(paperInScene));
            }
            if (infoButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(infoButton));
            }
        }

        /// <summary>
        /// Opens the additional information window.
        /// </summary>
        public void OnOpenClick()
        {
            infoButton.SetActive(false);
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the additional information window.
        /// </summary>
        public void OnCloseClick()
        {
            this.gameObject.SetActive(false);
            infoButton.SetActive(true);
        }

        /// <summary>
        /// Tries to open the paper in browser.
        /// </summary>
        public void OnOpenInExporerClick()
        {
            Paper paper = paperInScene.GetComponent<PaperDataDisplay>().Content;
            string url = paper.URL;
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
        }
    }

}
