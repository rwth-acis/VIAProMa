using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Input;

public class CommandStackManager : Singleton<CommandStackManager>
{
    public Stack undoActionStack;
    public Stack redoActionStack;

    private void OnEnable()
    {
        undoActionStack = new Stack();
        redoActionStack = new Stack();
    }

    public void UndoToolAction()
    {
        IToolAction action = (IToolAction)undoActionStack.Pop();
        if (action != null)
        {
            action.UndoAction();
            redoActionStack.Push(action);
        }
    }

    public void RedoToolAction()
    {
        IToolAction action = (IToolAction)redoActionStack.Pop();
        if (action != null)
        {
            action.DoAction();
            undoActionStack.Push(action);
        }
    }
}
