using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.ProgressBars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Visualizations.BuildingProgressBar;

public class ProgressBarTester : MonoBehaviour
{
    [Range(0, 1)]
    public float percentDone = 0f;
    [Range(0, 1)]
    public float percentInProgress = 0f;

    public int buildingIndex = 0;

    public GameObject progressBar;

    private IProgressBarVisuals progressBarVisuals;

    private void Awake()
    {
        if (progressBar == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBar));
        }
        progressBarVisuals = progressBar.GetComponent<IProgressBarVisuals>();
        if(progressBar.GetComponent<BuildingProgressBarVisuals>())
        {
            progressBar.GetComponent<BuildingProgressBarVisuals>().BuildingModelIndex = buildingIndex;
        }
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
