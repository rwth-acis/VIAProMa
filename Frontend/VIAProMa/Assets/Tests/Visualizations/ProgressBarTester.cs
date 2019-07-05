using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarTester : MonoBehaviour
{
    [Range(0, 1)]
    public float percentDone = 0f;
    [Range(0, 1)]
    public float percentInProgress = 0f;

    public ProgressBarController progressBar;

    private void Awake()
    {
        if (progressBar == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBar));
        }
    }

    // Update is called once per frame
    void Update()
    {
        progressBar.PercentageDone = percentDone;
        progressBar.PercentageInProgress = percentInProgress;
    }
}
