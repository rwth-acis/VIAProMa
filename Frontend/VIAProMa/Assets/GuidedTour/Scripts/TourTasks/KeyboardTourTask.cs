// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
using UnityEngine;
// using Microsoft.MixedReality.Toolkit.UI;
using i5.VIAProMa.UI.KeyboardInput;
using i5.VIAProMa.UI.InputFields;

namespace GuidedTour {
    public class KeyboardTourTask : AbstractTourTask
    {
        private bool done = false;
        [SerializeField] private InputField inputField;
        [SerializeField] private string targetString = "helloWorld.url";
        [SerializeField] private Keyboard keyboard;
        
        private void Awake() 
        {
            keyboard = keyboard.gameObject.GetComponent<Keyboard>();
            keyboard.InputFinished += c_KeyboardClosed;
            Active = true;
        }

        void c_KeyboardClosed(object sender, InputFinishedEventArgs args) {
            if (args.Aborted == false) {
                Debug.Log("Keyboard-Close-Event was listened to successfully");
                OnAction();
            }
        }

        internal override void SkipTask() 
        {
            if(Active && !done) 
            {
                // keyboard.Text = targetString;
                // keyboard.InputDone(false);
                Debug.Log("Successfully skipped Input");
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
        internal override bool IsTaskDone() 
        {
            return done;
        }

        internal void OnAction() 
        {
            if (Active && !done) 
            {
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
}
