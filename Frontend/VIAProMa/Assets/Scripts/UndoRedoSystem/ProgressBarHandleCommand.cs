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

    public void Execute()
    {
        
    }

    public void Undo()
    {
        progressBar.StartResizing(finPointer, newHandleOnPositiveCap);
        progressBar.SetHandles(prevPointer, newHandleOnPositiveCap);
    }

    public void Redo()
    {
        progressBar.StartResizing(prevPointer, newHandleOnPositiveCap);
        progressBar.SetHandles(finPointer, newHandleOnPositiveCap);
    }
}