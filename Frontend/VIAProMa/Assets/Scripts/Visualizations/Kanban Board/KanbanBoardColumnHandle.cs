using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanbanBoardColumnHandle : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField] private KanbanBoardColumnVisualController kanbanBoardController;

    public bool xAxis;
    public bool positiveEnd;

    private IMixedRealityPointer activePointer;
    private Vector3 pointerStartPosition;
    private Vector3 kanbanBoardColumnStartPosition;
    private float startLength;

    private void Awake()
    {
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
            float handDelta;
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
                float newLength = startLength + handDelta;
                float previousWidth = kanbanBoardController.Width;
                kanbanBoardController.Width = newLength;
                if (kanbanBoardController.Width != previousWidth) // only move if the width was actually changed (it could be unaffected if min or max size was reached)
                {
                    Vector3 pivotCorrection = new Vector3(handDelta / 2f, 0, 0);
                    if (positiveEnd)
                    {
                        pivotCorrection *= -1;
                    }
                    kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition - kanbanBoardController.transform.localRotation * pivotCorrection;
                }
            }
            else
            {
                float newLength = startLength + handDelta;
                float previousHeight = kanbanBoardController.Height;
                kanbanBoardController.Height = newLength;
                if (kanbanBoardController.Width != previousHeight) // only move if the height was actually changed (it could be unaffected if min or max size was reached)
                {
                    Vector3 pivotCorrection = new Vector3(0, handDelta / 2f, 0);
                    if (positiveEnd)
                    {
                        pivotCorrection *= -1;
                    }
                    kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition - kanbanBoardController.transform.localRotation * pivotCorrection;
                }
            }

            // mark pointer data as used
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
