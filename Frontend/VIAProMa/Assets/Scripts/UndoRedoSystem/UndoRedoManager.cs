using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager of the Undo-Redo-System.
/// </summary>
public class UndoRedoManager : MonoBehaviour
{
    private CommandProcessor commandProcessor;

    void Start()
    {
        commandProcessor = new CommandProcessor();
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Executes the given command and integrates it into the Undo-Redo System.
    /// </summary>
    /// <param name="command">
    /// Command which will be executed.
    /// </param>
    public void Execute(ICommand command)
    {
        commandProcessor.Execute(command);
    }

    /// <summary>
    /// Reverses the last command.
    /// </summary>
    public void Undo()
    {
        commandProcessor.Undo();
    }

    /// <summary>
    /// Repeats the next command.
    /// </summary>
    public void Redo()
    {
        commandProcessor.Redo();
    }

    /* -------------------------------------------------------------------------- */

    public List<ICommand> getCommandList()
    {
        return commandProcessor.getCommandListCP();
    }

    public int getCurrentPosition()
    {
        return commandProcessor.getCurrentPosition();
    }

    public void setCurrentPosition(int pCurrPos)
    {
        commandProcessor.setCurrentPosition(pCurrPos);
    }
}