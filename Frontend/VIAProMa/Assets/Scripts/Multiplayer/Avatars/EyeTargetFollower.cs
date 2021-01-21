using i5.VIAProMa.Utilities;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars
{
    public class EyeTargetFollower : MonoBehaviour
    {
        private Transform headBone;

        public Transform eyeTarget;
        public float minHorizontalRotation;
        public float maxHorizontalRotation;
        public float minVerticalRotation;
        public float maxVerticalRotation;

        private void Awake()
        {
            if (eyeTarget == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(eyeTarget));
            }
            headBone = transform.parent;
        }

        void Update()
        {
            Vector3 eyeToTarget = eyeTarget.position - transform.position;
            Vector3 projectedForward = new Vector3(0, 0, -1);

            Vector3 horizontalForward = Vector3.ProjectOnPlane(-transform.right, headBone.up);
            Vector3 horizontalEyeToTarget = Vector3.ProjectOnPlane(eyeToTarget, headBone.up);
            float horizontalAngle = Vector3.SignedAngle(projectedForward, horizontalEyeToTarget, headBone.up);

            Vector3 verticalForward = Vector3.ProjectOnPlane(-transform.right, headBone.right);
            Vector3 verticalEyeToTarget = Vector3.ProjectOnPlane(eyeToTarget, headBone.right);
            float verticalAngle = Vector3.SignedAngle(projectedForward, verticalEyeToTarget, headBone.right);

            transform.localEulerAngles = new Vector3(
                Mathf.Clamp(verticalAngle, minVerticalRotation, maxVerticalRotation) - 90f, // correction because the bone starts at -90
                Mathf.Clamp(horizontalAngle, minHorizontalRotation, maxHorizontalRotation),
                0f
                );
        }
    }
}