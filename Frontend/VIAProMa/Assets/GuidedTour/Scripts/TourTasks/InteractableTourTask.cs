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
     * Attach this script to a "Task" game-object and drag the gameobject with the "<see cref="Interactable"/>" component into  
     * the "<see cref="interactable"/>" field in the inspector.
     * </summary>
     */
    public class InteractableTourTask : AbstractTourTask
    {
        [SerializeField] internal Interactable interactable;
        [SerializeField] private GameObject highlighter;
        [SerializeField] private bool isHighlighterNeeded;
        [SerializeField] private HighlighterDirection arrowPosition;
        [SerializeField] private Vector3 customOffset = new Vector3(0, 0, 0);

        internal bool done = false;
        private GameObject objectHighlighter;
        private InteractableTaskEventHandler tourTaskEventHandler;
        private enum HighlighterDirection { arrowOnLeft, arrowOnTop, arrowOnRight, arrowOnBottom};

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
