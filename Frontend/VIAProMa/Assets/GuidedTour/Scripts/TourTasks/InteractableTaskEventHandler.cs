﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;


namespace GuidedTour
{

    /**
     * <summary>
     * This listens to, or causes, "OnClick"-Events of gameobjects with the "Interactable" component.
     * An instance of this Script will automatically attatch itself to any InteractableTourTask.
     * </summary>
     */

    [UnityEngine.AddComponentMenu("Scripts/MRTK/SDK/InteractableReceiver")]
    public class InteractableTaskEventHandler : ReceiverBaseMonoBehavior
    {
        private InteractableTourTask task;

        private void Awake() 
        {
            task = GetComponent<InteractableTourTask>();
            Interactable = task.interactable;
        }

        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = default) 
        {
            if (task.Active == true && task.done == false) 
            {
                Debug.Log("InteractableTourTask was successfully finished");
                task.done = true;
                Destroy(this);
            }
        }

        internal void EmulateButtonPress() 
        {
            if (task.Active == true && task.done == false) 
            {
                Debug.Log("InteractableTourTask was successfully skipped");
                Interactable.TriggerOnClick();
                task.done = true;
                Destroy(this);
            }
        }
    }
}