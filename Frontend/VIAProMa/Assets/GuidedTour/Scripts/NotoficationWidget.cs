using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

namespace GuidedTour
{
    public class NotoficationWidget : MonoBehaviour
    {
        public GameObject notificationWidget;
        public TextMeshPro notificationPreviewLabel;

        public bool CanShowMessages { get; set; } = true;

        public void ShowMessage(string text)
        {
            if (CanShowMessages)
            {
                gameObject.SetActive(true);
                notificationPreviewLabel.text = text;
            }
        }

        public void HideMessage()
        {
            notificationPreviewLabel.text = "";
            gameObject.SetActive(false);
        }


    }
}
