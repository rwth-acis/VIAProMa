using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] private Transform capPos;
    [SerializeField] private Transform capNeg;
    [SerializeField] private Transform tubes;
    [SerializeField] private Transform innerBarDone;
    [SerializeField] private Transform innerBarInProgress;

    public float minLength = 0.05f;

    public float Length { get => tubes.localScale.x; }

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
    }

    public void SetLength(bool manipulationOnPosCap, float newLength)
    {
        // make sure that the minLength in positive
        // the minLength may never be 0 because otherwise we cannot scale up again
        if (minLength <= 0)
        {
            minLength = 0.001f;
        }

        // ensure that the newLength is greater than minLength
        if (newLength < minLength)
        {
            newLength = minLength;
        }


        float relativeScale = newLength / Length;
        Vector3 objectPosRelativeToPivot = new Vector3(Length / 2f, 0, 0);
        if (manipulationOnPosCap)
        {
            objectPosRelativeToPivot *= -1f;
        }

        Vector3 pivotPosition = transform.localPosition - transform.localRotation * objectPosRelativeToPivot;

        transform.localPosition = (relativeScale * (transform.localRotation * objectPosRelativeToPivot)) + pivotPosition;


        tubes.localScale = new Vector3(newLength, 1f, 1f);
        capPos.localPosition = new Vector3(newLength / 2f, 0f, 0f);
        capNeg.localPosition = new Vector3(-newLength / 2f, 0f, 0f);
    }
}
