using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSpineController : MonoBehaviour
{
    [SerializeField] private Transform chestBone;
    [SerializeField] private Transform neckBone;
    [SerializeField] private Transform headBone;
    [SerializeField] private Transform eyeLeftBone;
    [SerializeField] private Transform eyeRightBone;

    public Vector3 position;
    public Vector3 eulerAngles;
    public Vector3 maximumHeadNeckAngles;

    private Quaternion initialChestRotation;
    private Quaternion initialNeckRotation;
    private Quaternion initialHeadRotation;
    private Vector3 eyeMiddle;
    private Vector3 chestToEyesOffset;
    private Vector3 lastPosition;

    private void Awake()
    {
        if (chestBone == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(chestBone));
        }
        if (neckBone == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(neckBone));
        }
        if (headBone == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(headBone));
        }
        if (eyeLeftBone == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(eyeLeftBone));
        }
        if (eyeRightBone == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(eyeRightBone));
        }

        eyeMiddle = Vector3.Lerp(eyeLeftBone.localPosition, eyeRightBone.localPosition, 0.5f);
        initialHeadRotation = headBone.localRotation;
        initialNeckRotation = neckBone.localRotation;
        initialChestRotation = chestBone.localRotation;
    }

    private void Update()
    {
        Vector3 targetHeadNeckRotation = eulerAngles;
        Vector3 targetBodyRotation = Vector3.zero;

        if (Mathf.Abs(eulerAngles.x) > maximumHeadNeckAngles.x)
        {
            targetHeadNeckRotation.x = Mathf.Sign(eulerAngles.x) * maximumHeadNeckAngles.x;
            targetBodyRotation.x = Mathf.Sign(eulerAngles.x) * (Mathf.Abs(eulerAngles.x) - maximumHeadNeckAngles.x);
        }
        if (Mathf.Abs(eulerAngles.y) > maximumHeadNeckAngles.y)
        {
            targetHeadNeckRotation.y = Mathf.Sign(eulerAngles.y) * maximumHeadNeckAngles.y;
            targetBodyRotation.y = Mathf.Sign(eulerAngles.y) * (Mathf.Abs(eulerAngles.y) - maximumHeadNeckAngles.y);
        }
        if (Mathf.Abs(eulerAngles.z) > maximumHeadNeckAngles.z)
        {
            targetHeadNeckRotation.z = Mathf.Sign(eulerAngles.z) * maximumHeadNeckAngles.z;
            targetBodyRotation.z = Mathf.Sign(eulerAngles.z) * (Mathf.Abs(eulerAngles.z) - maximumHeadNeckAngles.z);
        }

        transform.localEulerAngles = targetBodyRotation;

        Quaternion rotation = Quaternion.Euler(targetHeadNeckRotation);

        neckBone.localRotation = Quaternion.Lerp(initialNeckRotation, initialNeckRotation * rotation, 0.5f);
        headBone.localRotation = Quaternion.Lerp(initialHeadRotation, initialHeadRotation * rotation, 0.5f);



        lastPosition = position;
    }
}
