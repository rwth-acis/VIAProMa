using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace i5.VIAProMa.UI.FootMenu
{
    public class HeightChanger : MonoBehaviour, IMixedRealityPointerHandler
    {
        private IMixedRealityPointer activePointer;
        private Vector3 startPosition;
        private float startHeight;

        public float maxMovement = 0.4f;

        public float Height { get; private set; }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (activePointer == null && !eventData.used)
            {
                activePointer = eventData.Pointer;
                startPosition = activePointer.Position;
                startHeight = Height;

                // Mark the pointer data as used to prevent other behaviors from handling input events
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                Vector3 delta = activePointer.Position - startPosition;
                float handDelta = Vector3.Dot(Vector3.up, delta);

                Height = startHeight + handDelta / maxMovement;

                // Mark the pointer data as used to prevent other behaviors from handling input events
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