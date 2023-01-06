using System.Collections.Generic;

public class CommandProcessor
{
    private List<ICommand> commands = new List<ICommand>();
    private int currentPosition = -1;

    public void Execute(ICommand command)
    {
        command.Execute();
        commands.Add(command);
        currentPosition++;
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
    }

    public void Redo()
    {
        if (currentPosition >= commands.Count - 1)
        {
            return;
        }

        currentPosition++;
        ICommand command = commands[currentPosition];
        command.Execute();
    }
}