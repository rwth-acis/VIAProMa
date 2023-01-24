using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CommandProcessor
{
    private List<ICommand> commands = new List<ICommand>();
    private int currentPosition = -1;
    private int range = 0;

    private Color notActiveColor = Color.grey;
    private Color activeColor;
    private GameObject undoButtonBG;
    private GameObject redoButtonBG;
    private GameObject closeButton;
    private int state = 0; // Used for only initializing references/colors once. Used instead of Awake() as objects are not initialized at this point

    public void Execute(ICommand command)
    {
        if(currentPosition < commands.Count - 1)
        {
            range = commands.Count - (currentPosition + 1); 
            commands.RemoveRange(currentPosition + 1, range);
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
        ChangeColor(true, false);

        //TODO Delete Debug.log
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
        
        //TODO Delete Debug.log
        Debug.Log(currentPosition);

        // Undo only possible if there is still something to undo, redo possible
        if (currentPosition == -1)
        {
            ChangeColor(false, true);
        } else
        {
            ChangeColor(true, true);
        }
    }

    public void Redo()
    {
        if (currentPosition >= commands.Count - 1)
        {
            return;
        }
        else if(currentPosition >= commands.Count - 2)
        {
            ChangeColor(true, false);
        }
        else
        {
            ChangeColor(true, true);
        }
        currentPosition++;
        Debug.Log(currentPosition);
        ICommand command = commands[currentPosition];
        command.Execute();
    }

    public void ChangeColor(bool undoable, bool redoable)
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