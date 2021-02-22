using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveActionUndoable : IToolAction
{
    public GameObject target;
    //Start 6 DOF
    public Vector3 startPosition;
    public Quaternion startRotation;

    //End 6 DOF
    public Vector3 endPosition;
    public Quaternion endRotation;

    void IToolAction.DoAction()
    {
        target.transform.SetPositionAndRotation(endPosition, endRotation);
    }

    void IToolAction.UndoAction()
    {
        endPosition = target.transform.position;
        endRotation = target.transform.rotation;
        target.transform.SetPositionAndRotation(startPosition, startRotation);
    }
}
