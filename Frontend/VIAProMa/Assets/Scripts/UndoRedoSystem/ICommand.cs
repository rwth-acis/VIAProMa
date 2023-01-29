public interface ICommand
{
    /// <description>
    /// For developers adding new functions to the Undo-Redo-System (Command Pattern):
    /// - an UndoRedoManager object has to exist in the scene where the system is used
    /// - for the function to undo/ redo:
    ///     - create a class inheriting the ICommand class
    ///     - create a constructor with parameters for the command containing all the variables that will be used in the Execute/Redo function
    ///     - implement an Execute function that will be executed when the command is used. This is just the normal behavior.
    ///     - implement an Undo function to define how undoing for this action specifically looks like.
    ///     - in the rest of the code where the action is performed (e.g. when clicking a button) instead of having the action logic,
    ///       instanciate a command with the correct parameters and execute this command using the UndoRedoManager. E.g: 
    ///         ICommand transform = new AppBarTransformCommand(startPosition, startRotation, startScale, appBarPlacer);
    ///         UndoRedoManager.Execute(transform);
    /// </description>


    /// <summary>
    /// Interface that has to be implemented by every command. In Execute the "normal" behavior of the action has to be defined.
    /// Because Execute and Redo have the same method body, only Execute is used in the CommandProcessors Redo function.
    /// </summary>
    void Execute();

    /// <summary>
    /// Here has to be defined what specific action will be performed for undoing a specific one.
    /// </summary>
    void Undo();
}