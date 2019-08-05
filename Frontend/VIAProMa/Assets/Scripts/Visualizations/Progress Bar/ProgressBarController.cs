using Microsoft.MixedReality.Toolkit.UI;
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
    [SerializeField] private CapsuleCollider tubeCollider;
    [SerializeField] private BoundingBox boundingBox;
    [SerializeField] private TextLabel textLabel;

    public float minLength = 0.05f;
    public float maxLength = 3f;

    private float percentageDone;
    private float percentageInProgress;

    private BoxCollider boundingBoxCollider;

    public float Length { get => tubes.localScale.x; }

    public string Title
    {
        get => textLabel.Text;
        set => textLabel.Text = value;
    }

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
        if (tubeCollider == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(tubeCollider));
        }
        if (boundingBox == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(boundingBox));
        }
        boundingBoxCollider = boundingBox?.gameObject.GetComponent<BoxCollider>();
        if (boundingBox == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBox), boundingBoxCollider?.gameObject);
        }

        if (textLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(textLabel));
        }

        UpdateVisuals();
        UpdateTextLabelPositioning(Length);
    }

    private void UpdateVisuals()
    {
        if (innerBarDone == null || innerBarInProgress == null)
        {
            return;
        }
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

        // also update box colliders and bounding box
        tubeCollider.height = newLength + 0.1f; // add 0.1 so that the cylindrical part covers the full length (otherwise it is too short because of the rounded caps)
        boundingBoxCollider.size = new Vector3(
            newLength + 0.05f, // add 0.05f to encapsulate the end caps
            boundingBoxCollider.size.y,
            boundingBoxCollider.size.z);
        boundingBox.Refresh();

        UpdateTextLabelPositioning(newLength);
    }

    private void UpdateTextLabelPositioning(float progressBarLength)
    {
        textLabel.MaxWidth = progressBarLength / 2f;
        textLabel.transform.localPosition = new Vector3(
            -progressBarLength / 2f + progressBarLength / 4f,
        textLabel.transform.localPosition.y,
            textLabel.transform.localPosition.z);
    }
}
