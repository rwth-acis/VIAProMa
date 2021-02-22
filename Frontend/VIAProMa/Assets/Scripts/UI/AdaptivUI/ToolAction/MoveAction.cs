using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// The parts of the move tool, that require MonoBehavior
/// </summary>
public class MoveAction : ActionHelperFunctions
{
    /// <summary>
    /// Create a MoveActionUndoable, save the corresponding data from the visualisation the tool currently points at in it and push it on the undo stack
    /// </summary>
    /// <param name="eventData"></param> The data from the corresponding input event
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

    /// <summary>
    /// Activates the BoundingBox from all visualisations in the scene
    /// </summary>
    public void StartAdjusting()
    {
        boundingBoxStateControllers = FindObjectsOfType<BoundingBoxStateController>();
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            boundingbox.BoundingBoxActive = true;
        }
    }

    /// <summary>
    /// Deactivates the BoundingBox that were activated by StartAdjusting()
    /// </summary>
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
