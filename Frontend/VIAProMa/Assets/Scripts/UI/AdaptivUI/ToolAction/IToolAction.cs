using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IToolAction
{
    void DoAction();
    void UndoAction();
}
