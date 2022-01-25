using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

namespace GuidedTour
{
    public class NotificationWidget : MonoBehaviour
    {
        public GameObject notificationWidget;
        public TextMeshPro notificationPreviewLabel;

        public void ShowMessage(string text, float seconds)
        {
            StopAllCoroutines();

            gameObject.SetActive(true);
            notificationPreviewLabel.text = text;

            StartCoroutine(Deactivate(seconds));
        }

        private IEnumerator Deactivate(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            gameObject.SetActive(false);
        }

        public void HideMessage()
        {
            notificationPreviewLabel.text = "";
            gameObject.SetActive(false);
        }


    }
}
