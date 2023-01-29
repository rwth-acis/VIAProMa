public interface ICommand
{
    /// <summary>
    /// Interface that has to be implemented by every command. In Execute the "normal" behavior of the action has to be defined.
    /// </summary>
    void Execute();

    /// <summary>
    /// Here has to be defined what specific action will be performed for undoing a specific one.
    /// </summary>
    void Undo();
}