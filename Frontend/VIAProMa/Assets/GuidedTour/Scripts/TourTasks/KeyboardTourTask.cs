// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
using UnityEngine;
// using Microsoft.MixedReality.Toolkit.UI;
using i5.VIAProMa.UI.KeyboardInput;
using i5.VIAProMa.UI.InputFields;
using System;
using System.Collections;

namespace GuidedTour
{
    public class KeyboardTourTask : AbstractTourTask
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private string targetString = "helloWorld.url";
        private Keyboard keyboard;
        private bool done = false;

        private void Awake()
        {
            keyboard = Resources.FindObjectsOfTypeAll<Keyboard>()[0];
            keyboard.InputFinished += c_KeyboardClosed;
        }

        void c_KeyboardClosed(object sender, InputFinishedEventArgs args)
        {
            if (args.Aborted == false)
            {
                Debug.Log("Keyboard-Close-Event was listened to successfully");
                OnAction();
            }
        }

        internal override void SkipTask()
        {
            if (State == TourTaskState.ACTIVE && !done)
            {
                keyboard.Text = targetString;
                keyboard.InputDone(false);
                Debug.Log("Successfully skipped Input");
                done = true;
            }
        }

        internal override bool IsTaskDone()
        {
            return done;
        }

        internal void OnAction()
        {
            if (State == TourTaskState.ACTIVE && !done)
            {
                StartCoroutine(CheckString());
            }
        }

        // Wait for a short time to ensure field is updated
        private IEnumerator CheckString()
        {
            yield return new WaitForSeconds(0.1f);

            if (inputField.ContentField.text.Equals(targetString))
            {
                Debug.Log("String is matching");
                done = true;
            }
            else
            {
                Debug.Log("String is not matching");
            }
        }
    }
}
