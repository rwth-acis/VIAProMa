using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class MoveAction : ActionHelperFunctions
{
    public void RecordPosition(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromGameObject(eventData.InputSource.Pointers[0].Result.CurrentPointerTarget);
        if (target != null)
        {
            MoveActionUndoable moveActionUndoable = new MoveActionUndoable();
            moveActionUndoable.target = target;
            moveActionUndoable.startPosition = target.transform.position;
            moveActionUndoable.startRotation = target.transform.rotation;
            CommandStackManager.Instance.undoActionStack.Push(moveActionUndoable);
        }
    }

    BoundingBoxStateController[] boundingBoxStateControllers;

    public void StartAdjusting()
    {
        boundingBoxStateControllers = FindObjectsOfType<BoundingBoxStateController>();
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            boundingbox.BoundingBoxActive = true;
        }
    }

    public void StopAdjusting()
    {
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (boundingbox != null)
            {
                boundingbox.BoundingBoxActive = false;
            }
        }
    }
}
