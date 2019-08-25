using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarSpineController))]
public class AvatarMovementSynchronizer : BasicAvatarMovementSynchronizer
{
    private AvatarSpineController spineController;

    protected override void Awake()
    {
        base.Awake();
        spineController = GetComponent<AvatarSpineController>();
    }

    protected override void Update()
    {
        if (photonView.IsMine)
        {
            // set the position based on the camera
            transform.position = mainCamera.transform.position;
            transform.rotation = mainCamera.transform.rotation;
            timeSinceLastSearch += Time.deltaTime;
        }
        else
        {
            spineController.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            spineController.targetRotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
            MoveAvatarHand(avatarLeftHand, leftHandTargetPosition);
            MoveAvatarHand(avatarRightHand, rightHandTargetPosition);
        }
    }
}
