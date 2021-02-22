using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The undo and redo action
/// </summary>
public class UndoActions : MonoBehaviour
{
    /// <summary>
    /// Undo the last action and put it on the redo stack
    /// </summary>
    public void UndoToolAction()
    { 
        if (CommandStackManager.Instance.undoActionStack.Count > 0)
        {
            IToolAction action = (IToolAction)CommandStackManager.Instance.undoActionStack.Pop();
            action.UndoAction();
            CommandStackManager.Instance.redoActionStack.Push(action);
        }
    }

    /// <summary>
    /// Redo the last action and put in on the undo stack
    /// </summary>
    public void RedoToolAction()
    {
        
        if (CommandStackManager.Instance.redoActionStack.Count > 0)
        {
            IToolAction action = (IToolAction)CommandStackManager.Instance.redoActionStack.Pop();
            action.DoAction();
            CommandStackManager.Instance.undoActionStack.Push(action);
        }
    }
}
