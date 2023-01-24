using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace i5.VIAProMa.Visualizations.KanbanBoard
{
    public class KanbanBoardColumnHandle : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private KanbanBoardColumnVisualController kanbanBoardController;

        public bool xAxis;
        public bool positiveEnd;

        private IMixedRealityPointer activePointer;
        private Vector3 pointerStartPosition;
        private Vector3 kanbanBoardColumnStartPosition;
        private Vector3 kanbanBoardColumnEndPosition;
        private float startLength;

        private GameObject CommandController;
        private CommandController commandController;
        
        private float oldHeight;
        private float oldWidth;
        private float newHeight;
        private float newWidth;
        
        float handDelta;
        float newLength;
        float previousWidth;
        float previousHeight;

        private void Awake()
        {
            CommandController = GameObject.Find("CommandController");
            commandController = CommandController.GetComponent<CommandController>();

            if (kanbanBoardController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(kanbanBoardController));
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
                pointerStartPosition = activePointer.Position;
                kanbanBoardColumnStartPosition = kanbanBoardController.transform.localPosition;
                if (xAxis)
                {
                    startLength = kanbanBoardController.Width;
                }
                else
                {
                    startLength = kanbanBoardController.Height;
                }

                // Mark pointer data as used
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                Vector3 delta = activePointer.Position - pointerStartPosition;
                if (xAxis)
                {
                    handDelta = Vector3.Dot(kanbanBoardController.transform.right, delta);
                }
                else
                {
                    handDelta = Vector3.Dot(kanbanBoardController.transform.up, delta);
                }
                if (!positiveEnd)
                {
                    handDelta *= -1f;
                }
                if (xAxis)
                {
                    newLength = startLength + handDelta;
                    oldWidth = startLength;
                    previousWidth = kanbanBoardController.Width;
                    kanbanBoardController.Width = newLength;
                    newWidth = newLength;
                    if (kanbanBoardController.Width != previousWidth) // only move if the width was actually changed (it could be unaffected if min or max size was reached)
                    {
                        Vector3 pivotCorrection = new Vector3(handDelta / 2f, 0, 0);
                        if (positiveEnd)
                        {
                            pivotCorrection *= -1;
                        }
                        kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition - kanbanBoardController.transform.localRotation * pivotCorrection;
                        kanbanBoardColumnEndPosition = kanbanBoardController.transform.localPosition;
                    }
                }
                else
                {
                    newLength = startLength + handDelta;
                    oldHeight = startLength;
                    previousHeight = kanbanBoardController.Height;
                    kanbanBoardController.Height = newLength;
                    newHeight = newLength;
                    if (kanbanBoardController.Height != previousHeight) // only move if the height was actually changed (it could be unaffected if min or max size was reached)
                    {
                        Vector3 pivotCorrection = new Vector3(0, handDelta / 2f, 0);
                        if (positiveEnd)
                        {
                            pivotCorrection *= -1;
                        }
                        kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition - kanbanBoardController.transform.localRotation * pivotCorrection;
                        kanbanBoardColumnEndPosition = kanbanBoardController.transform.localPosition;
                    }
                }

                // Mark pointer data as used
                eventData.Use();
            }
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            ICommand drag = new ScaleKanbanBoardCommand(kanbanBoardColumnStartPosition, xAxis, kanbanBoardController, oldWidth, oldHeight, newWidth, newHeight, kanbanBoardColumnEndPosition);
            commandController.Execute(drag);
            if (eventData.Pointer == activePointer && !eventData.used)
            {
                activePointer = null;
                eventData.Use();
            }
        }

        private void MoveForPivotScaling()
        {
            Vector3 objToPivot;
            if (xAxis)
            {
                objToPivot = new Vector3(kanbanBoardController.Width, 0, 0);
            }
            else
            {
                objToPivot = new Vector3(0, kanbanBoardController.Height, 0);
            }
            if (!positiveEnd)
            {
                objToPivot *= -1;
            }
        }
    }
}