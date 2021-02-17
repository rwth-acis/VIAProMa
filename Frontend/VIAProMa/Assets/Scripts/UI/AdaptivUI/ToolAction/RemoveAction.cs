using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class RemoveAction : ActionHelperFunctions
{
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
