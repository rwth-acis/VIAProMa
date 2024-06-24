using UnityEngine;
using i5.VIAProMa.Visualizations.ProgressBars;

/// <summary>
/// Command which allows dragging of the Progress Bar.
/// </summary>
public class ProgressBarHandleCommand : ICommand
{
    private Vector3 prevPointer;
    private Vector3 finPointer;

    private bool newHandleOnPositiveCap;
    private ProgressBarController progressBar;

    public ProgressBarHandleCommand(Vector3 sPointer, Vector3 sFinalPointer, bool sNewHandleOnPositiveCap, ProgressBarController sProgressBar)
    {
        prevPointer = sPointer;
        finPointer = sFinalPointer;
        newHandleOnPositiveCap = sNewHandleOnPositiveCap;
        progressBar = sProgressBar;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Due to a special case the method is left empty and instead Redo() is used.
    /// </summary>
    public void Execute()
    {

    }

    /// <summary>
    /// Reverses the resizing of a progress bar.
    /// </summary>
    public void Undo()
    {
        progressBar.StartResizing(finPointer, newHandleOnPositiveCap);
        progressBar.SetHandles(prevPointer, newHandleOnPositiveCap);
    }

    /// <summary>
    /// Repeats the resizing of a progress bar.
    /// </summary>
    public void Redo()
    {
        progressBar.StartResizing(prevPointer, newHandleOnPositiveCap);
        progressBar.SetHandles(finPointer, newHandleOnPositiveCap);
    }

    /* -------------------------------------------------------------------------- */

    public ProgressBarController getProgressBarController()
    {
        return progressBar;
    }
}