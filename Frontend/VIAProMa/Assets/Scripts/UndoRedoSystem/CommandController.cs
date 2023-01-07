using UnityEngine;

public class CommandController : MonoBehaviour
{
    private CommandProcessor commandProcessor;

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
}
