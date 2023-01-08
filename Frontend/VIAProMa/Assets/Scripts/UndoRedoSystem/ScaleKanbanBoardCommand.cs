using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Visualizations.KanbanBoard;

public class ScaleKanbanBoardCommand : ICommand
{
    public bool xAxis;
    public bool positiveEnd;

    private IMixedRealityPointer activePointer;
    private Vector3 pointerStartPosition;
    private Vector3 kanbanBoardColumnStartPosition;
    private float startLength;
    private MixedRealityPointerEventData eventData;
    private i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController kanbanBoardController;

    public ScaleKanbanBoardCommand(MixedRealityPointerEventData seventData, float sstartlength, Vector3 skanbanBoardColumnStartPosition, Vector3 spointerStartPosition, IMixedRealityPointer dactivePointer, bool sxAxis, bool spositiveEnd, i5.VIAProMa.Visualizations.KanbanBoard.KanbanBoardColumnVisualController skanbanBoardController)
    {
        xAxis = sxAxis;
        positiveEnd = spositiveEnd;
        activePointer = dactivePointer;
        pointerStartPosition = spointerStartPosition;
        kanbanBoardColumnStartPosition = skanbanBoardColumnStartPosition;
        startLength = sstartlength;
        eventData = seventData;
        kanbanBoardController = skanbanBoardController;

    }

    public void Execute()
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
                if (kanbanBoardController.Height != previousHeight) // only move if the height was actually changed (it could be unaffected if min or max size was reached)
                {
                    Vector3 pivotCorrection = new Vector3(0, handDelta / 2f, 0);
                    if (positiveEnd)
                    {
                        pivotCorrection *= -1;
                    }
                    kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition - kanbanBoardController.transform.localRotation * pivotCorrection;
                }
            }

        }

        // mark pointer data as used
        eventData.Use();
    }



    //versucht, an den Rechenoperationen rumzubasteln fuer redo: funktioniert nicht
        public void Undo()
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
                // original: + statt -
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
                    // original: - statt +
                    kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition + kanbanBoardController.transform.localRotation * pivotCorrection;
                }
            }
            else
            {
                float newLength = startLength + handDelta;
                float previousHeight = kanbanBoardController.Height;
                kanbanBoardController.Height = newLength;
                if (kanbanBoardController.Height != previousHeight) // only move if the height was actually changed (it could be unaffected if min or max size was reached)
                {
                    Vector3 pivotCorrection = new Vector3(0, handDelta / 2f, 0);
                    if (positiveEnd)
                    {
                        pivotCorrection *= -1;
                    }
                    //original: - statt +
                    kanbanBoardController.transform.localPosition = kanbanBoardColumnStartPosition + kanbanBoardController.transform.localRotation * pivotCorrection;
                }
            }

        }

        // mark pointer data as used
        eventData.Use();
    }
}
