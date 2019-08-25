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

    [SerializeField] private bool useDebugRotation;
    [SerializeField] private Vector3 targetEulerAngles;

    public Vector3 position;
    [HideInInspector] public Quaternion targetRotation;
    public float bodyFollowSpeed = 1f;

    private Quaternion initialNeckRotation;
    private Quaternion initialHeadRotation;
    private Vector3 eyeMiddleLocalPosition;
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

        eyeMiddleLocalPosition = Vector3.Lerp(eyeLeftBone.localPosition, eyeRightBone.localPosition, 0.5f);
        initialHeadRotation = headBone.localRotation;
        initialNeckRotation = neckBone.localRotation;
    }

    private void Update()
    {
        if (useDebugRotation)
        {
            targetRotation = Quaternion.Euler(targetEulerAngles);
        }

        // determine the rotations of neck, head and body:
        // difference between current base rotation and target rotation
        Quaternion headNeckTargetRotation = Quaternion.Inverse(transform.localRotation) * targetRotation;
        // apply rotation half and half to neck and head
        neckBone.localRotation = Quaternion.Slerp(Quaternion.identity, headNeckTargetRotation, 0.5f) * initialNeckRotation;
        headBone.localRotation = Quaternion.Slerp(Quaternion.identity, headNeckTargetRotation, 0.5f) * initialHeadRotation;
        // apply lerping rotation to the body => the body will slowly follow the head back into the relaxed position
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, bodyFollowSpeed * Time.deltaTime);

        // determine the position:
        Vector3 offset = transform.position - headBone.TransformPoint(eyeMiddleLocalPosition);
        transform.localPosition = position + offset;

        // alternative using hand midpoints

        //Vector3 targetLookPos = handsMidpoint.position;
        //targetLookPos.y = transform.position.y;

        //transform.LookAt(targetLookPos);
        //transform.Rotate(new Vector3(0, 180, 0));

        //Quaternion rotation = Quaternion.Euler(eulerAngles) * Quaternion.Inverse(transform.localRotation);
        //neckBone.localRotation = Quaternion.Lerp(initialNeckRotation, initialNeckRotation * rotation, 0.5f);
        //headBone.localRotation = Quaternion.Lerp(initialHeadRotation, initialHeadRotation * rotation, 0.5f);


        // alternative using rotation limits:

        //Vector3 targetHeadNeckRotation = eulerAngles;
        //Vector3 targetBodyRotation = Vector3.zero;

        //if (Mathf.Abs(eulerAngles.x) > maximumHeadNeckAngles.x)
        //{
        //    targetHeadNeckRotation.x = Mathf.Sign(eulerAngles.x) * maximumHeadNeckAngles.x;
        //    targetBodyRotation.x = Mathf.Sign(eulerAngles.x) * (Mathf.Abs(eulerAngles.x) - maximumHeadNeckAngles.x);
        //}
        //if (Mathf.Abs(eulerAngles.y) > maximumHeadNeckAngles.y)
        //{
        //    targetHeadNeckRotation.y = Mathf.Sign(eulerAngles.y) * maximumHeadNeckAngles.y;
        //    targetBodyRotation.y = Mathf.Sign(eulerAngles.y) * (Mathf.Abs(eulerAngles.y) - maximumHeadNeckAngles.y);
        //}
        //if (Mathf.Abs(eulerAngles.z) > maximumHeadNeckAngles.z)
        //{
        //    targetHeadNeckRotation.z = Mathf.Sign(eulerAngles.z) * maximumHeadNeckAngles.z;
        //    targetBodyRotation.z = Mathf.Sign(eulerAngles.z) * (Mathf.Abs(eulerAngles.z) - maximumHeadNeckAngles.z);
        //}

        //transform.localEulerAngles = targetBodyRotation;

        //Quaternion rotation = Quaternion.Euler(targetHeadNeckRotation);

        //neckBone.localRotation = Quaternion.Lerp(initialNeckRotation, initialNeckRotation * rotation, 0.5f);
        //headBone.localRotation = Quaternion.Lerp(initialHeadRotation, initialHeadRotation * rotation, 0.5f);
    }
}
