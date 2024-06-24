using UnityEngine;

/// <summary>
/// Command which allows moving of Object.
/// </summary>
public class MoveObjectCommand : ICommand
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject gameObject;

    public MoveObjectCommand(Vector3 pStartPosition, Vector3 pEndPosition, GameObject pGameObject)
    {
        startPosition = pStartPosition;
        endPosition = pEndPosition;
        gameObject = pGameObject;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Moves an object.
    /// </summary>
    public void Execute()
    {
        gameObject.transform.localPosition = endPosition;
    }

    /// <summary>
    /// Reverses the moving of an object.
    /// </summary>
    public void Undo()
    {
        gameObject.transform.localPosition = startPosition;
    }
}