using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.KanbanBoard;
using i5.VIAProMa.ResourceManagagement;


public class DeleteObjectCommand : ICommand
{
    private GameObject gameObjectAppBar;
    private GameObject gameObject;

    /// <summary>
    /// Creates a command for deleting an object.
    /// </summary>
    /// <param name="givenGameObject"></param>
    /// <param name="givenAppBar"></param>
    public DeleteObjectCommand(GameObject givenGameObject, GameObject givenAppBar) 
    {
        gameObjectAppBar = givenAppBar;
        gameObject = givenGameObject;
    }

    /// <summary>
    /// Deletes an object by setting in inactive.
    /// </summary>
    public void Execute()
    {
        if(gameObjectAppBar != null)
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

