using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectCommand : ICommand
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject gameObject;

    /// <summary>
    /// Creates a command for moving objects.
    /// </summary>
    /// <param name="pStartPosition"></param>
    /// <param name="pEndPosition"></param>
    /// <param name="pgameObject"></param>
    public MoveObjectCommand(Vector3 pStartPosition, Vector3 pEndPosition, GameObject pgameObject)
    {
        startPosition = pStartPosition;
        endPosition = pEndPosition;
        gameObject = pgameObject;
    }

    /// <summary>
    /// Moves an object.
    /// </summary>
    public void Execute()
    {
        gameObject.transform.localPosition = endPosition;
    }

    /// <summary>
    /// Undos the moving of an object.
    /// </summary>
    public void Undo()
    {
        gameObject.transform.localPosition = startPosition;
    }
}
