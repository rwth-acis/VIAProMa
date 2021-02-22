using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoActions : MonoBehaviour
{
    public void UndoToolAction()
    {
        
        if (CommandStackManager.Instance.undoActionStack.Count > 0)
        {
            IToolAction action = (IToolAction)CommandStackManager.Instance.undoActionStack.Pop();
            action.UndoAction();
            CommandStackManager.Instance.redoActionStack.Push(action);
        }
    }

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
