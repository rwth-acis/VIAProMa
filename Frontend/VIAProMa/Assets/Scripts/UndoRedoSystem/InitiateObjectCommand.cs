using UnityEngine;

/// <summary>
/// Command which initiates shelves and its objects.
/// </summary>
public class InitiateObjectCommand : ICommand
{
    private GameObject initiatedObject;
    private GameObject objectAppbar;

    public InitiateObjectCommand(GameObject instance, GameObject instanceAppBar)
    {
        initiatedObject = instance;
        objectAppbar = instanceAppBar;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Initiates the object.
    /// </summary>
    public void Execute()
    {
        if(objectAppbar != null)
        {
            objectAppbar.SetActive(true);

        }
        initiatedObject.SetActive(true);
    }

    /// <summary>
    /// Deletes the object.
    /// </summary>
    public void Undo()
    {
        initiatedObject.SetActive(false);
        if (objectAppbar != null)
        {
            objectAppbar.SetActive(false);
        }
    }
}