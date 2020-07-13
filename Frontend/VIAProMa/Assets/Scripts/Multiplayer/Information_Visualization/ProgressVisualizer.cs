using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressVisualizer : MonoBehaviour
{
    //public float percentDone = 0f;
    //public float percentInProgress = 0f;

    public GameObject progressBar;
    private IProgressBarVisuals progressBarVisuals;
    public TextAsset jsonFile;

    private void Start()
    {
        Information MentorData = JsonUtility.FromJson<Information>(jsonFile.text);

        float percentDone = MentorData.average_score/100f;
        float percentInProgress = 1f - percentDone;

        progressBarVisuals = progressBar.GetComponent<IProgressBarVisuals>();

        progressBarVisuals.PercentageDone = percentDone;
        progressBarVisuals.PercentageInProgress = percentInProgress;

    }


    /*
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
    */
}
