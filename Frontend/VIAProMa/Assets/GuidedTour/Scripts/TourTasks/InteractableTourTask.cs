using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;


namespace GuidedTour
{
    public class InteractableTourTask : AbstractTourTask
    {

        public Interactable interactable;
        InteractableTaskEventHandler tourTaskEventHandler;
        internal bool done = false;


        // Start is called before the first frame update
        void Start() 
        {
            tourTaskEventHandler = gameObject.AddComponent<InteractableTaskEventHandler>();
        }

        internal override void SkipTask() 
        {
            tourTaskEventHandler.EmulateButtonPress();
        }

        public void TemporarySkipTask() 
        {
            tourTaskEventHandler.EmulateButtonPress();
        }

        internal override bool IsTaskDone() 
        {
            return done;
        }
    }
}
