using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Visualizations.KanbanBoard;
using i5.VIAProMa.Visualizations.ProgressBars;

public class ProgressBarHandleCommand : ICommand
{
    private Vector3 previousPosition;
    private bool newHandleOnPositiveCap;
    private ProgressBarController progressBar;

    public ProgressBarHandleCommand(Vector3 spreviousPosition, bool snewHandleOnPositiveCap, ProgressBarController sprogressBar)
    {
        previousPosition = spreviousPosition;
        newHandleOnPositiveCap = !snewHandleOnPositiveCap;
        progressBar = sprogressBar;
        
    }

    public void Execute()
    {

    }

    public void Undo()
    {
        progressBar.SetHandles(previousPosition, newHandleOnPositiveCap);
    }
}
