using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarTester : MonoBehaviour
{
    public bool scaleOnPosSide;
    public float length = 1f;

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
        progressBar.SetLength(scaleOnPosSide, length);
    }
}
