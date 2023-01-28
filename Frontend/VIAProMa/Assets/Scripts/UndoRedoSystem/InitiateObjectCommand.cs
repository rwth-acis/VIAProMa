using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateObjectCommand : ICommand
{
    private GameObject initiatedObject;

    public InitiateObjectCommand(GameObject instance)
    {
        initiatedObject = instance;
    }

        public void Execute()
    {
        initiatedObject.SetActive(true);
    }

    public void Undo()
    {
        initiatedObject.SetActive(false);
    }
}
