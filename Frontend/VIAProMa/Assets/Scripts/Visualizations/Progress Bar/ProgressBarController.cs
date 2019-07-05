using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarController : MonoBehaviour, IProgressBarVisuals
{
    [SerializeField] private Transform capPos;
    [SerializeField] private Transform capNeg;
    [SerializeField] private Transform tubes;
    [SerializeField] private Transform innerBarDone;
    [SerializeField] private Transform innerBarInProgress;

    public float minLength = 0.05f;
    public float maxLength = 3f;

    private float percentageDone;
    private float percentageInProgress;

    public float Length { get => tubes.localScale.x; }

    public float PercentageDone
    {
        get => percentageDone;
        set
        {
            percentageDone = Mathf.Clamp01(value);
            UpdateVisuals();
        }
    }

    public float PercentageInProgress
    {
        get => percentageInProgress;
        set
        {
            percentageInProgress = Mathf.Clamp01(value);
            UpdateVisuals();
        }
    }

    private void Awake()
    {
        if (capPos == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(capPos));
        }
        if (capNeg == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(capNeg));
        }
        if (tubes == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(tubes));
        }
        if (innerBarDone == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(innerBarDone));
        }
        if (innerBarInProgress == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(innerBarInProgress));
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        float doneBarScale = percentageDone;
        float inProgressBarScale = percentageInProgress;

        if (percentageDone == 0)
        {
            doneBarScale = 0.001f;
        }
        if (percentageInProgress == 0)
        {
            inProgressBarScale = 0.001f;
        }
        else
        {
            inProgressBarScale = Mathf.Clamp(percentageInProgress, 0, 1 - percentageDone);
        }

        innerBarDone.localScale = new Vector3(doneBarScale, 1f, 1f);
        innerBarInProgress.localPosition = new Vector3(-0.5f + doneBarScale, 0f, 0f);
        innerBarInProgress.localScale = new Vector3(inProgressBarScale, 1f, 1f);
    }

    public void SetLength(bool manipulationOnPosCap, float newLength)
    {
        // make sure that the minLength in positive
        // the minLength may never be 0 because otherwise we cannot scale up again
        if (minLength <= 0)
        {
            minLength = 0.001f;
        }

        // ensure that the newLength is between minLength and maxLength
        newLength = Mathf.Clamp(newLength, minLength, maxLength);

        // move the object so that scaling operation has its pivot at one of the caps
        float relativeScale = newLength / Length;
        Vector3 objectPosRelativeToPivot = new Vector3(transform.localScale.x * Length / 2f, 0, 0);
        if (!manipulationOnPosCap)
        {
            objectPosRelativeToPivot *= -1f;
        }

        Vector3 pivotPosition = transform.localPosition - transform.localRotation * objectPosRelativeToPivot;

        transform.localPosition = (relativeScale * (transform.localRotation * objectPosRelativeToPivot)) + pivotPosition;

        // scale the tubes to the new length and update the caps' position
        tubes.localScale = new Vector3(newLength, 1f, 1f);
        capPos.localPosition = new Vector3(newLength / 2f, 0f, 0f);
        capNeg.localPosition = new Vector3(-newLength / 2f, 0f, 0f);
    }
}
