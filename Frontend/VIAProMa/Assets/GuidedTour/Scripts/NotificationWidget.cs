﻿using System.Collections;
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
        [SerializeField] private GameObject notificationWidget;
        [SerializeField] private TextMeshPro notificationPreviewLabel;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip succuess;
        [SerializeField] private AudioClip fail;

        /**
         * <summary>Show a notification</summary>
         * <param name="text">The text to display</param>
         * <param name="seconds">The time to display the notification in seconds</param>
         */
        public void ShowMessage(string text, float seconds)
        {
            // Stop running coroutines if another message was showing up before
            StopAllCoroutines();

            gameObject.SetActive(true);
            notificationPreviewLabel.text = text;

            StartCoroutine(Deactivate(seconds));
        }

        // Wait for seconds and deactivate
        private IEnumerator Deactivate(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            gameObject.SetActive(false);
        }

        /**
         * <summary>Play the success audio clip</summary>
         */
        public void PlaySuccessSound()
        {
            audioSource.clip = succuess;
            audioSource.Play();
        }

        /**
         * <summary>Play the fail audio clip</summary>
         */
        public void PlayFailSound()
        {
            audioSource.clip = fail;
            audioSource.Play();
        }

    }
}
