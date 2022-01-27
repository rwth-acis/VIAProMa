// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
using UnityEngine;
// using Microsoft.MixedReality.Toolkit.UI;
using i5.VIAProMa.UI.KeyboardInput;
using i5.VIAProMa.UI.InputFields;
using System;
using System.Collections;

namespace GuidedTour {

    /**
     * <summary>
     * This creates a tour-task, that requires the user to interact with an input-field and type in a string.
     * Attach this script to a "Task" game-object and drag the desired input-field into "Input Field" in the inspector.
     * </summary>
     */
    public class KeyboardTourTask : AbstractTourTask
    {
        private bool done = false;
        [SerializeField] private InputField inputField;
        [SerializeField] private string targetString = "";
        private Keyboard keyboard;

        /**
         * <summary>
         * On its creation, the task will find the keyboard and subscribe to the InputFinish-event of the keyboard.
         * This way we can determine, when the user has stopped their input and closed the keyboard.
         * </summary>
         */
        private void Awake() 
        {
            keyboard = Resources.FindObjectsOfTypeAll<Keyboard>()[0];
            keyboard.InputFinished += c_KeyboardClosed;
        }

        /**
         * <summary>
         * This method is called when a button has been clicked, that closed the keyboard.
         * If the user clickes the x button, the input is aborted and the string won't be evaluated.
         * Only if the input has been confirmed by clicking the confirm-button, OnAction is called.
         * </summary>
         */
        void c_KeyboardClosed(object sender, InputFinishedEventArgs args) {
            if (args.Aborted == false) {
                OnAction();
            }
        }

        /**
         * <summary>
         * With this method, the keyboardTourTask can be skipped. This will automatically enter the desired string into the
         * the input-field.
         * </summary>
         */
        internal override void SkipTask() 
        {
            if(State == TourTaskState.ACTIVE && !done) 
            {
                keyboard.Text = targetString;
                keyboard.InputDone(false);
                done = true;
            }
        }

        public void AutoCompleteInput() 
        {
            keyboard.Text = targetString;
            //keyboard.InputDone(false);
        }

        /*
        public void temp() 
        {
            if (Active && !done) 
            {
                foreach(char a in targetString) {
                    keyboard.AddLetter(a);
                }
                // keyboard.InputDone(false);
                Debug.Log("Successfully skipped Input");
                done = true;
            }
        }
        */

        /**
         * <summary>
         * Functionality required by the GuidedTourManager.
         * If the task is finished, the bool done will be set to true, so that the manager can load the next one.
         * </summary>
         */
        internal override bool IsTaskDone() 
        {
            return done;
        }

        /**
         * <summary>
         * This will be called when the confirm-button of the keyboard has been clicked.
         * It will start the CheckString-Coroutine to check if the input was correct. 
         * </summary>
         */
        internal void OnAction() 
        {
            if (State == TourTaskState.ACTIVE && !done) 
            {
                StartCoroutine(CheckString());
            }
        }


        /**
         * <summary>
         * This checks if the user has typed in the correct input.
         * If they have, then the Task is set to done and the GuidedTourManager will select the next one.
         * </summary>
         */
        private IEnumerator CheckString()
        {
            // Wait for a short time to ensure field is updated
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

        internal override void OnTaskActivation(GameObject indicatorArrow) {

        }
        internal override void OnTaskDeactivation(GameObject indicatorArrow) { }
    }
}
