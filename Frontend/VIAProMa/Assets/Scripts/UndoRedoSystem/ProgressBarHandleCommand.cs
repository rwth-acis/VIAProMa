using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Visualizations.KanbanBoard;
using i5.VIAProMa.Visualizations.ProgressBars;

public class ProgressBarHandleCommand : ICommand
{
    private Vector3 prevPointer;
    private Vector3 finPointer;

    private bool newHandleOnPositiveCap;
    private ProgressBarController progressBar;

    public ProgressBarHandleCommand(Vector3 spointer, Vector3 finalPointer, bool snewHandleOnPositiveCap, ProgressBarController sprogressBar)
    {
        prevPointer = spointer;
        finPointer = finalPointer;
        newHandleOnPositiveCap = snewHandleOnPositiveCap;
        progressBar = sprogressBar;
    }

    /// <summary>
    /// Due to a special case the method is _________________
    /// </summary>
    public void Execute()
    {
        
    }

    /// <summary>
    /// Undos the resizing of a progress bar.
    /// </summary>
    public void Undo()
    {
        progressBar.StartResizing(finPointer, newHandleOnPositiveCap);
        progressBar.SetHandles(prevPointer, newHandleOnPositiveCap);
    }

    /// <summary>
    /// Redos the resizing of a progress bar.
    /// </summary>
    public void Redo()
    {
        progressBar.StartResizing(prevPointer, newHandleOnPositiveCap);
        progressBar.SetHandles(finPointer, newHandleOnPositiveCap);
    }

    public ProgressBarController getProgressBarController()
    {
        return progressBar;
    }
}