using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The implementation of the IToolAction interface for the move tool
/// </summary>
public class MoveActionUndoable : IToolAction
{
    public GameObject target;
    //Start 6 DOF
    public Vector3 startPosition;
    public Quaternion startRotation;

    //End 6 DOF
    public Vector3 endPosition;
    public Quaternion endRotation;

    /// <summary>
    /// Move the target to the end position and rotation
    /// </summary>
    void IToolAction.DoAction()
    {
        target.transform.SetPositionAndRotation(endPosition, endRotation);
    }

    /// <summary>
    /// Save the current position in the end position and move the target to the start position and rotation
    /// </summary>
    void IToolAction.UndoAction()
    {
        endPosition = target.transform.position;
        endRotation = target.transform.rotation;
        target.transform.SetPositionAndRotation(startPosition, startRotation);
    }
}
