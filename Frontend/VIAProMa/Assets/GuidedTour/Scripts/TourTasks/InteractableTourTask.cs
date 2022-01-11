using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;


namespace GuidedTour
{

    /**
     * <summary>
     * This creates a tour-task, that requires the user to interact with an existing button in the scene.
     * Attatch this script to a "Task" game-object and drag the gameobject with the "Interactable" component into  
     * the "interactable" field in the inspector.
     * </summary>
     */

    public class InteractableTourTask : AbstractTourTask
    {

        
        public Interactable interactable;
        
        //This listenes to 
        InteractableTaskEventHandler tourTaskEventHandler;

        internal bool done = false;


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
