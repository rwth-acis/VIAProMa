using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

namespace GuidedTour
{

    /**
     * <summary>
     * This creates a tour-task, that requires the user to interact with an existing button in the scene.
     * Attach this script to a "Task" game-object and drag the gameobject with the "Interactable" component into  
     * the "interactable" field in the inspector.
     * </summary>
     */
    public class InteractableTourTask : AbstractTourTask
    {

        
        public Interactable interactable;
        InteractableTaskEventHandler tourTaskEventHandler;
        internal bool done = false;
        private GameObject objectHighlighter;
        private enum highlighterDirection { arrowOnLeft, arrowOnTop, arrowOnRight, arrowOnBottom};
        [SerializeField] private GameObject highlighter;
        [SerializeField] private bool isHighlighterNeeded;
        [SerializeField] private highlighterDirection arrowPosition;
        [SerializeField] private Vector3 customOffset = new Vector3(0,0,0);

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

        internal override void OnTaskActivation(GameObject indicatorArrow) {
            if (indicatorArrow.activeInHierarchy == false) {
                indicatorArrow.SetActive(true);
            }
            indicatorArrow.GetComponent<DirectionalIndicator>().DirectionalTarget = interactable.gameObject.transform;

            if (isHighlighterNeeded) {
                objectHighlighter = Instantiate(highlighter, interactable.gameObject.transform);
                objectHighlighter.GetComponent<HighlighterAnimation>().HighlighterSetUP((int)arrowPosition, interactable.gameObject, customOffset);
            }
        }

        internal override void OnTaskDeactivation(GameObject indicatorArrow) {
            indicatorArrow.SetActive(false);
            indicatorArrow.GetComponent<DirectionalIndicator>().DirectionalTarget = null;
            Destroy(objectHighlighter);
        }
    }
}
