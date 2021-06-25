using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    public class MinimapHandle : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private MinimapController minimap;
        private IMixedRealityPointer activePointer;

        private Vector3 pointerStartPos;
        private Vector3 minimapStartPos;
        private float startLength;

        [Tooltip("Check this for handles that extend over the X-axis (else Z-axis is assumed)")]
        [SerializeField] 
        private bool xAxis;
        [SerializeField]
        private bool positiveEnd;

        private void Awake()
        {
            if (minimap is null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(minimap));
            }
        }

        // User clicks the handle
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (activePointer == null && !eventData.used)
            {
                activePointer = eventData.Pointer;
                pointerStartPos = activePointer.Position;
                minimapStartPos = minimap.transform.localPosition;

                if (xAxis)
                {
                    startLength = minimap.Width;
                }
                else
                {
                    startLength = minimap.Height;
                }

                eventData.Use();
            }
        }

        // User holds down mouse and moves cursor
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                // offset vector for new handle position
                Vector3 delta = activePointer.Position - pointerStartPos;
                float handleDelta;

                // x axis => transform left/right
                if (xAxis)
                {
                    handleDelta = Vector3.Dot(minimap.transform.right, delta);
                }
                // z axis => transform top/bottom
                else
                {
                    handleDelta = Vector3.Dot(minimap.transform.forward, delta);
                }

                if (!positiveEnd)
                {
                    handleDelta *= -1f;
                }

                float newLength = startLength + handleDelta;
                if (xAxis)
                {
                    float previousWidth = minimap.Width;
                    minimap.Width = newLength;

                    if (minimap.Width != previousWidth)
                    {
                        Vector3 pivotCorrection = new Vector3(handleDelta / 2f, 0, 0);
                        if (positiveEnd)
                        {
                            pivotCorrection *= -1;
                        }

                        minimap.transform.localPosition =
                            minimapStartPos - minimap.transform.localRotation * pivotCorrection;
                    }
                }
                else
                {
                    float previousHeight = minimap.Height;
                    minimap.Height = newLength;

                    if (minimap.Height != previousHeight)
                    {
                        Vector3 pivotCorrection = new Vector3(0f, 0f, handleDelta / 2f);
                        if (positiveEnd)
                        {
                            pivotCorrection *= -1;
                        }

                        //minimap.transform.localScale =
                        //    minimapStartPos; // - minimap.transform.localRotation * pivotCorrection;
                    }
                }

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

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }
    }
}