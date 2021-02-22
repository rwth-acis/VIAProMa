using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// The parts of the remove tool, that require MonoBehavior
/// </summary>
public class RemoveAction : ActionHelperFunctions
{
    /// <summary>
    /// Deactivate the object, the tool currently points at and push the corresponding action on the undo stack
    /// </summary>
    /// <param name="eventData"></param>
    public void RemoveVisualisation(BaseInputEventData eventData)
    {
        RemoveActionUndoable action = new RemoveActionUndoable();
        action.target = GetVisualisationFromGameObject(eventData.InputSource.Pointers[0].Result.CurrentPointerTarget);
        if (action.target != null)
        {
            ((IToolAction)action).DoAction();
            CommandStackManager.Instance.undoActionStack.Push(action);
        }
    }
}
