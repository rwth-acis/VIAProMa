using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.ProgressBars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarTester : MonoBehaviour
{
    [Range(0, 1)]
    public float percentDone = 0f;
    [Range(0, 1)]
    public float percentInProgress = 0f;

    public GameObject progressBar;

    private IProgressBarVisuals progressBarVisuals;

    private void Awake()
    {
        if (progressBar == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBar));
        }
        progressBarVisuals = progressBar.GetComponent<IProgressBarVisuals>();
    }

    // Update is called once per frame
    void Update()
    {
        progressBarVisuals.PercentageDone = percentDone;
        progressBarVisuals.PercentageInProgress = percentInProgress;
    }

    private void OnValidate()
    {
        if (progressBar != null)
        {
            progressBarVisuals = progressBar.GetComponent<IProgressBarVisuals>();
            if (progressBarVisuals == null)
            {
                progressBar = null;
            }
        }
    }
}
