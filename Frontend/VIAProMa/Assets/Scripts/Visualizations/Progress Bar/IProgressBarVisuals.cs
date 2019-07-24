using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProgressBarVisuals
{
    float PercentageDone { get; set; }
    float PercentageInProgress { get; set; }

    void SetTitle(string title);
}
