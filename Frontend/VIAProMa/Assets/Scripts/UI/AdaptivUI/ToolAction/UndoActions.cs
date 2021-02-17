using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoActions : MonoBehaviour
{
    public void UndoToolAction()
    {
        IToolAction action = (IToolAction)CommandStackManager.Instance.undoActionStack.Pop();
        if (action != null)
        {
            action.UndoAction();
            CommandStackManager.Instance.redoActionStack.Push(action);
        }
    }

    public void RedoToolAction()
    {
        IToolAction action = (IToolAction)CommandStackManager.Instance.redoActionStack.Pop();
        if (action != null)
        {
            action.DoAction();
            CommandStackManager.Instance.undoActionStack.Push(action);
        }
    }
}
