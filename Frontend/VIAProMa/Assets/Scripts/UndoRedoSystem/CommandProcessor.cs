using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CommandProcessor
{
    private List<ICommand> commands = new List<ICommand>();
    private int currentPosition = -1;

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

        Debug.Log(currentPosition);
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