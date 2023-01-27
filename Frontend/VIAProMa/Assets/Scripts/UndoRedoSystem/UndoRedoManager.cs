using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class UndoRedoManager : MonoBehaviour
{
    private CommandProcessor commandProcessor;

    void Start()
    {
        commandProcessor = new CommandProcessor();
    }

    public List<ICommand> getCommandList()
    {
        return commandProcessor.getCommandListCP();
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
}
