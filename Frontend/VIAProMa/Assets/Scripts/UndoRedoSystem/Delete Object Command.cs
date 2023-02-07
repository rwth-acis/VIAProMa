using UnityEngine;

/// <summary>
/// Command which deletes shelves and its objects.
/// </summary>
public class DeleteObjectCommand : ICommand
{
    private GameObject gameObjectAppBar;
    private GameObject gameObject;

    public DeleteObjectCommand(GameObject givenGameObject, GameObject givenAppBar)
    {
        gameObjectAppBar = givenAppBar;
        gameObject = givenGameObject;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Deletes an object by setting it inactive.
    /// </summary>
    public void Execute()
    {
        if (gameObjectAppBar != null)
        {
            gameObjectAppBar.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the deleted object to active.
    /// </summary>
    public void Undo()
    {
        if (gameObjectAppBar != null)
        {
            gameObjectAppBar.SetActive(true);
        }
        gameObject.SetActive(true);

    }
}