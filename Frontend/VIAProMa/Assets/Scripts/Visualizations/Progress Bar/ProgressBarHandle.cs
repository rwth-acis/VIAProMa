using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ProgressBars
{
    public class ProgressBarHandle : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private ProgressBarController progressBar;

        public bool handleOnPositiveCap;

        private IMixedRealityPointer activePointer;

        private Vector3 prevPointer;
        private Vector3 finPointer;

        private GameObject CommandController;
        private CommandController commandController;

        private void Awake()
        {
            CommandController = GameObject.Find("CommandController");
            commandController = CommandController.GetComponent<CommandController>();
            
            if (progressBar == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBar));
            }
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (activePointer == null && !eventData.used)
            {
                activePointer = eventData.Pointer;
                prevPointer = activePointer.Position;

                progressBar.StartResizing(activePointer.Position, handleOnPositiveCap);
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                progressBar.SetHandles(activePointer.Position, handleOnPositiveCap);
                eventData.Use();
            }     
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            finPointer = activePointer.Position;

            if (eventData.Pointer == activePointer && !eventData.used)
            {
                activePointer = null;
                eventData.Use();
            }

            ICommand resize = new ProgressBarHandleCommand(prevPointer, finPointer, handleOnPositiveCap, progressBar);
            commandController.Execute(resize);
        }
    }
}