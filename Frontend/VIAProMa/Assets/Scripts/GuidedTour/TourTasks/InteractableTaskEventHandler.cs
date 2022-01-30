using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;


namespace GuidedTour
{

    /**
     * <summary>
     * This listens to, or causes, "OnClick"-Events of gameobjects with the "<see cref="Interactable"/>" component.
     * An instance of this Script will automatically attach itself to any InteractableTourTask.
     * </summary>
     */
    [UnityEngine.AddComponentMenu("Scripts/MRTK/SDK/InteractableReceiver")]
    public class InteractableTaskEventHandler : ReceiverBaseMonoBehavior
    {
        private InteractableTourTask task;

        /**
         * <summary>
         * Once this script is activated, it gets the Interactable-component it should listen to, from the InteractableTourTask  
         * </summary>
         */
        private void Awake()
        {
            task = GetComponent<InteractableTourTask>();
            Interactable = task.interactable;
        }

        /**
         * <summary>
         * This describes what happens when the button is clicked.
         * If the task was active, it will be set to done, so that the GuidedTourManager can choose the next task.
         * </summary>
         */
        public override void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = default)
        {
            if (task.State == AbstractTourTask.TourTaskState.ACTIVE)
            {
                task.manager.OnTaskDone();
                Destroy(this);
            }
        }

        /**
         * <summary>
         * This will trigger an onClick-event. By calling this, you can skip a task.
         * </summary>
         */
        internal void EmulateButtonPress()
        {
            if (task.State == AbstractTourTask.TourTaskState.ACTIVE == true)
            {
                Interactable.TriggerOnClick();
                Destroy(this);
            }
        }
    }
}