using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CommandProcessor
{
    private List<ICommand> commands = new List<ICommand>();
    private int currentPosition = -1;

    private Color notActiveColor = Color.grey;
    private Color activeColor;
    public Material buttonMaterial;
    private GameObject undoButtonBG;
    private GameObject redoButtonBG;
    private GameObject closeButton;
    private int state = 0;


    public void Execute(ICommand command)
    {


        if(currentPosition < commands.Count - 1)
        {
            commands.RemoveRange(currentPosition + 1, commands.Count - 1);

        }


        commands.Add(command);
        currentPosition++;
        command.Execute();

        if (state == 0)
        {
            closeButton = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Close Button/BackPlate/Quad");
            activeColor = closeButton.GetComponent<Renderer>().material.color;
            undoButtonBG = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Backdrop/Undo Button/BackPlate/Quad");
            redoButtonBG = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Backdrop/Redo Button/BackPlate/Quad");
            state = 1;
        }

        // Undo is now possible, Redo not
        changeColor(true, false);
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
        Debug.Log(currentPosition);


        //Undo only possible if there is still something to undo, redo possible
        if (currentPosition == -1)
        {

            changeColor(false, true);
        } else
        {
            changeColor(true, true);
        }

    }

    public void Redo()
    {
        if (currentPosition >= commands.Count - 1)
        {
            return;
        }

        currentPosition++;
        Debug.Log(currentPosition);
        ICommand command = commands[currentPosition];
        command.Redo();

        if(currentPosition >= commands.Count - 1)
        {
            changeColor(true, false);
        }
        else
        {
            changeColor(true, true);
        }

    }

    public void changeColor(bool undoable, bool redoable)
    {
        if (!undoable)
        {
            undoButtonBG.GetComponent<Renderer>().material.color = notActiveColor;
        }
        if (!redoable)
        {
            redoButtonBG.GetComponent<Renderer>().material.color = notActiveColor;
        }
        if (undoable)
        {
            undoButtonBG.GetComponent<Renderer>().material.color = activeColor;
        }
        if (redoable)
        {
            redoButtonBG.GetComponent<Renderer>().material.color = activeColor;
        }
    }
}