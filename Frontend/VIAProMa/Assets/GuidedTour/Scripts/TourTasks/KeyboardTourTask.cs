using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

namespace GuidedTour {
    public class KeyboardTourTask : AbstractTourTask
    {
        private bool done = false;
        [SerializeField] private string targetString = "helloWorld.url";
        [SerializeField] private GameObject textArea;
        private TextMeshPro input;
        public Interactable interactable;

        private void Awake() 
        {
            input = textArea.GetComponent<TextMeshPro>();
        }

        internal override void SkipTask() 
        {
            if(Active && !done) {
                input.SetText(targetString);
                interactable.TriggerOnClick();
                done = true;
            }
        }

        public void temp() 
        {
            if (Active && !done) 
            {
                input.SetText(targetString);
                interactable.TriggerOnClick();
                done = true;
            }
        }

        internal override bool IsTaskDone() 
        {
            return done;
        }
        public void OnAction() 
        {
            if (Active && !done) 
            {
                if (input.text.Equals(targetString)) 
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
