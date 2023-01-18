using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CommandProcessor
{
    private List<ICommand> commands = new List<ICommand>();
    private int currentPosition = -1;

    private Color notActiveColor = Color.grey;
    private Color activeColor;
    private Material buttonMaterial;
    private GameObject undoButton;
    private GameObject redoButton;
    public void Awake()
    {

        buttonMaterial = Resources.Load("Material/Night_Sky.mat", typeof(Material)) as Material;
        activeColor = buttonMaterial.color;

    }

    public void Execute(ICommand command)
    {


        if(currentPosition < commands.Count - 1)
        {
            commands.RemoveRange(currentPosition + 1, commands.Count - 1);

        }


        commands.Add(command);
        currentPosition++;
        command.Execute();
        Debug.Log(currentPosition);
    }

    public void Undo()
    {
        if (currentPosition < 0)
        {
            return;
        }

        ICommand command = commands[currentPosition];
        command.Undo();
        currentPosition--;

        if(currentPosition == -1)
        {
            undoButton = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Backdrop/Undo Button/BackPlate/Quad");
            undoButton.GetComponent<Renderer>().material.color = notActiveColor;
        }

        if(currentPosition + 1 < commands.Count)
        {
            redoButton = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Backdrop/Redo Button/BackPlate/Quad");
            redoButton.GetComponent<Renderer>().material.color = activeColor;
        }
    }

    public void Redo()
    {
        if (currentPosition >= commands.Count - 1)
        {
            return;
        }

        currentPosition++;
        ICommand command = commands[currentPosition];
        command.Redo();
    }
}