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

    public DeleteObjectCommand(GameObject givenGameObject, GameObject givenAppBar) {
    gameObjectAppBar = givenAppBar;
    gameObject = givenGameObject;
    }


    public void Execute()
    {
        if(gameObjectAppBar != null)
        {
            gameObjectAppBar.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void Undo()
    {
        if (gameObjectAppBar != null)
        {
            gameObjectAppBar.SetActive(true);
        }
        gameObject.SetActive(true);

    }
}

