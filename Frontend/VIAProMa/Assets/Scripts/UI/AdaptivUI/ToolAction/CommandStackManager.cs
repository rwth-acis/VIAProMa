using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// Manages the undo and redo stack for the tool actions
/// </summary>
public class CommandStackManager : Singleton<CommandStackManager>
{
    public Stack undoActionStack;
    public Stack redoActionStack;

    private void OnEnable()
    {
        undoActionStack = new Stack();
        redoActionStack = new Stack();
    }
}
