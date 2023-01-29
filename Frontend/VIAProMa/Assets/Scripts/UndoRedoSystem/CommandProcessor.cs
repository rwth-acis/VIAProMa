using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandProcessor
{
    public List<ICommand> commands = new List<ICommand>();
    private int currentPosition = -1;
    private int range = 0;
    private Color notActiveColor = Color.grey;
    private Color activeColor;
    private GameObject undoButtonBG;
    private GameObject redoButtonBG;
    private GameObject closeButton;
    private string activeSceneName;

    /// <summary>
    /// Constructor of the class. The buttons will be greyed out if there is nothing to be undone/redone.
    /// The paths that are set are dependent on whether the menu is tested in the correspondig testscene or used in the mainscene.
    /// </summary>
    public CommandProcessor()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        activeSceneName = activeScene.name;
        if (activeSceneName == "MainScene")
        {
            closeButton = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Close Button/BackPlate/Quad");
            undoButtonBG = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Backdrop/Undo Button/BackPlate/Quad");
            redoButtonBG = GameObject.Find("AnchorParent/Managers/Window Manager/UndoRedoMenu(Clone)/Leiste/Backdrop/Redo Button/BackPlate/Quad");
        }
        else if (activeSceneName == "UndoRedoTest")
        {
            closeButton = GameObject.Find("UndoRedoMenu/Leiste/Close Button/BackPlate/Quad");
            undoButtonBG = GameObject.Find("UndoRedoMenu/Leiste/Backdrop/Undo Button/BackPlate/Quad");
            redoButtonBG = GameObject.Find("UndoRedoMenu/Leiste/Backdrop/Redo Button/BackPlate/Quad");
        }
        activeColor = closeButton.GetComponent<Renderer>().material.color;
        RefreshColor();
    }

    /// <summary>
    /// Returns the commandlist.
    /// </summary>
    /// <returns></returns>
    public List<ICommand> getCommandListCP()
    {
        return commands;
    }

    /// <summary>
    /// Executes the given command. In case multiple actions have been undone and a new action is performed, the ones after the current action are removed to prevent complications.
    /// </summary>
    /// <param name="command"></param>
    public void Execute(ICommand command)
    {
        if (currentPosition < commands.Count - 1)
        {
            range = commands.Count - (currentPosition + 1); 
            commands.RemoveRange(currentPosition + 1, range);
        }
        commands.Add(command);
        currentPosition++;
        command.Execute();
        RefreshColor();
    }

    /// <summary>
    /// Undos an action and adjusts the color of the button.
    /// </summary>
    public void Undo()
    {
        if (currentPosition < 0)
        {
            RefreshColor();
            return;
        }
        ICommand command = commands[currentPosition];
        command.Undo();
        currentPosition--;
        RefreshColor();
    }

    /// <summary>
    /// Redos an action and adjusts the color of the button.
    /// The second if-statement is Special case for the ProgressBarHandleCommand, where instead of Execute() Redo() is used.
    /// This is done because the dragging of the Handle Bar is continous, while the Redo snaps it to the last position the Handle Bar was let go off.
    /// </summary>
    public void Redo()
    {
        if (currentPosition >= commands.Count - 1)
        {
            RefreshColor();
            return;
        }
        currentPosition++;
        ICommand command = commands[currentPosition];
        if (command.GetType() == typeof(ProgressBarHandleCommand))
        {
            ProgressBarHandleCommand progressBarHandleCommand = (ProgressBarHandleCommand)command;
            progressBarHandleCommand.Redo();
        } 
        command.Execute();
        RefreshColor();
    }

    /// <summary>
    ///  Sets the color of the Undo and Redo Button, indicating if they are active or inactive.
    /// </summary>
    /// <param name="undoable">State of the Undo Button. Active (blue) if true, inactive (grey) if false</param>
    /// <param name="redoable">State of the Redo Button. Active (blue) if true, inactive (grey) if false</param>
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
    
    /// <summary>
    /// Determines whether the color of the buttons have to be changed.
    /// </summary>
    public void RefreshColor()
    {
        bool redoPossible = (currentPosition <= commands.Count - 2 && commands.Count >= 1);
        bool undoPossible = (currentPosition >= 0);
        ChangeColor(undoPossible, redoPossible);
    }

}