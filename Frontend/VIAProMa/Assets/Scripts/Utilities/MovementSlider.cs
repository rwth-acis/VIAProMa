using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace i5.VIAProMa.Utilities
{
    public class MovementSlider : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private SlideDirection sliderDirection;

        public UnityEvent OnInteractionStarted;
        public UnityEvent OnInteractionEnded;
        public UnityEvent OnInteractionUpdated;

        private IMixedRealityPointer activePointer;
        private Vector3 startPointerPosition;
        private Vector3 startSliderPosition;

        public Vector3 DirectionVector
        {
            get
            {
                if (sliderDirection == SlideDirection.X_AXIS)
                {
                    return transform.right;
                }
                else if (sliderDirection == SlideDirection.Y_AXIS)
                {
                    return transform.up;
                }
                else
                {
                    return transform.forward;
                }
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
                startPointerPosition = activePointer.Position;
                startSliderPosition = gameObject.transform.position;
                OnInteractionStarted?.Invoke();
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                Vector3 delta = activePointer.Position - startPointerPosition;
                float handDelta = Vector3.Dot(DirectionVector, delta);

                transform.position = startSliderPosition + handDelta * DirectionVector;
                OnInteractionUpdated?.Invoke();

                eventData.Use();
            }
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                OnInteractionEnded?.Invoke();
                activePointer = null;

                eventData.Use();
            }
        }
    }

    public enum SlideDirection
    {
        X_AXIS, Y_AXIS, Z_AXIS
    }
}