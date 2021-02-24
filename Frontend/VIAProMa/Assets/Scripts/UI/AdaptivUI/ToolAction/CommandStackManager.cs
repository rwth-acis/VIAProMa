using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;

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

    public void CleanUp()
    {
        while (undoActionStack.Count > 0)
        {
            RemoveActionUndoable action = undoActionStack.Pop() as RemoveActionUndoable;
            if (action != null)
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Destroy(action.target);
                }
                else
                {
                    Destroy(action.target);
                }
            }
        }
        undoActionStack = new Stack();
        redoActionStack = new Stack();
    }
}
