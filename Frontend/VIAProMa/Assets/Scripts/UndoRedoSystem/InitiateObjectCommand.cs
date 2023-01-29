using UnityEngine;

/// <summary>
/// Command which initiates shelves and its objects.
/// </summary>
public class InitiateObjectCommand : ICommand
{
    private GameObject initiatedObject;

    public InitiateObjectCommand(GameObject instance)
    {
        initiatedObject = instance;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Initiates the object.
    /// </summary>
    public void Execute()
    {
        initiatedObject.SetActive(true);
    }

    /// <summary>
    /// Deletes the object.
    /// </summary>
    public void Undo()
    {
        initiatedObject.SetActive(false);
    }
}