using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class UndoRedoManager : MonoBehaviour
{
    private CommandProcessor commandProcessor;

    /// <summary>
    /// Used to have easier access to the Command Processor.
    /// </summary>
    void Start()
    {
        commandProcessor = new CommandProcessor();
    }

    public void Execute(ICommand command)
    {
        commandProcessor.Execute(command);
    }

    public void Undo()
    {
        commandProcessor.Undo();
    }

    public void Redo()
    {
        commandProcessor.Redo();
    }

    public List<ICommand> GetCommandList()
    {
        return commandProcessor.getCommandListCP();

    }
}
