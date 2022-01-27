using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine.Serialization;
using UnityEditor;

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
        // Needs to be set when creating an interactable tour task
        public Interactable interactable;

        // This will listen to or cause the OnClick-Events of the Interactable-Component
        private InteractableTaskEventHandler tourTaskEventHandler;

        internal bool done = false;
        

        // The highlighter is an optional arrow that can point towards the Interactable-Button.
        // You can change it's relative position or offset in the inspector.
        // In most situations the standard offset should be sufficient.
        private GameObject objectHighlighter;

        private enum HighlighterDirection { arrowOnLeft, arrowOnTop, arrowOnRight, arrowOnBottom };
        [Header("Highlighter Arrows")]
        [SerializeField] private GameObject highlighter;
        [SerializeField] private bool isHighlighterNeeded;
        [SerializeField] private HighlighterDirection arrowPosition;
        [SerializeField] private Vector3 indicatorOffset = new Vector3(0,0,0);


        // The input blocker is an optional cube, that covers interactable tour task buttons that have not yet been activated by the GuidedTour manager
        // It should automatically cover the size of the button, but if additional sizing or offset is required, you can set them in the inspector.
        [Header("Input Blocker")]
        [SerializeField] private bool isblockerNeeded;
        [SerializeField] private GameObject inputBlockerPrefab;
        [SerializeField] private Vector3 inputBlockerOffset;
        [SerializeField] private Vector3 inputBlockerScale = new Vector3(0.01f,0.01f,0.01f);
        private GameObject inputBlocker;
        private Vector3 inputBlockerBounds;

        /**
         * <summary>
         * When the app is launched:
         * Creates the Eventhandler for this specific InteractableTourTask.
         * Sets the input-blocker for when the task has not yet been activated by the GuidedTourManager
         * </summary>
         */
        void Start() 
        {   
            tourTaskEventHandler = gameObject.AddComponent<InteractableTaskEventHandler>();
            
            if (isblockerNeeded) {
                if (this.State == AbstractTourTask.TourTaskState.SCHEDULED) {
                    inputBlocker = Instantiate(inputBlockerPrefab, interactable.gameObject.transform.position + inputBlockerScale, interactable.gameObject.transform.rotation);
                    inputBlockerBounds = interactable.gameObject.GetComponent<Collider>().bounds.size;
                    inputBlocker.transform.localScale = new Vector3(inputBlockerBounds.x + inputBlockerScale.x, inputBlockerBounds.y + inputBlockerScale.y, inputBlockerBounds.z + inputBlockerScale.z);

                }
            }
        }

        /**
         * <summary>
         * Makes sure, that the input-blocker stays in the correct position.
         * </summary>
         */
        private void Update() {

            if (inputBlocker != null) {
                inputBlocker.transform.position = interactable.gameObject.transform.position;

            }
        }

        /**
         * <summary>
         * Skip a task, by sending an OnClick-event to the Interactable-Component of the Button. 
         * </summary>
         */
        internal override void SkipTask() 
        {
            tourTaskEventHandler.EmulateButtonPress();
        }

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
         * This is called by the GuidedTourManager, when this InteractableTourTask is activated.
         * When the task is activated, destroy the inputBlocker object, so that the Button can be clicked.
         * Activate the indicatorArrows that appear, when the user is turned too far away from the Button.
         * Activate the highlighterArrow that sits next to the button and points directly towards it.
         * </summary>
         */
        internal override void OnTaskActivation(GameObject indicatorArrow) {

            if (inputBlocker != null) {
                Destroy(inputBlocker);
            }

            if (indicatorArrow.activeInHierarchy == false) {
                indicatorArrow.SetActive(true);
            }

            indicatorArrow.GetComponent<DirectionalIndicator>().DirectionalTarget = interactable.gameObject.transform;

            if (isHighlighterNeeded) {
                objectHighlighter = Instantiate(highlighter, interactable.gameObject.transform);
                objectHighlighter.GetComponent<HighlighterAnimation>().HighlighterSetUP((int)arrowPosition, interactable.gameObject, indicatorOffset);
            }
        }

        /**
         * <summary>
         * This is called by the GuidedTourManager, when this InteractableTourTask is deactivated.
         * Disables the indicatorArrows that appear, when the user is turned too far away from the Button.
         * Destroys the highlighterArrow that sits next to the button and points directly towards it.
         * </summary>
         */
        internal override void OnTaskDeactivation(GameObject indicatorArrow) {
            indicatorArrow.SetActive(false);
            indicatorArrow.GetComponent<DirectionalIndicator>().DirectionalTarget = null;
            if (objectHighlighter != null) {
                Destroy(objectHighlighter);
            }
        }
    }
}
