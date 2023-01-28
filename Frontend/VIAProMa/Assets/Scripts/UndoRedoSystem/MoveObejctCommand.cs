using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectCommand : ICommand
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject gameObject;

    public MoveObjectCommand(Vector3 pStartPosition, Vector3 pEndPosition, GameObject pgameObject)
    {
        startPosition = pStartPosition;
        endPosition = pEndPosition;
        gameObject = pgameObject;
    }

    public void Execute()
    {
        gameObject.transform.localPosition = endPosition;
    }

    public void Undo()
    {
        gameObject.transform.localPosition = startPosition;
    }
}
