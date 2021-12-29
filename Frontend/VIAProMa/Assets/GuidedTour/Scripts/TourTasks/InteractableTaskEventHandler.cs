using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;


namespace GuidedTour
{
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
                Debug.Log("Clicked by Luk lol");
                task.done = true;
                Destroy(this);
            }
        }

        internal void EmulateButtonPress() 
        {
            if (task.Active == true && task.done == false) 
            {
                Interactable.TriggerOnClick();
                task.done = true;
                Destroy(this);
            }
        }
    }
}