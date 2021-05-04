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

        private void Awake()
        {
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
                progressBar.StartResizing(handleOnPositiveCap,activePointer.Position);
                // Mark pointer data as used
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
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                activePointer = null;
                eventData.Use();
            }
        }
    }
}