using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Common
{
    public class TransformSynchronizer : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private bool synchronizePosition = true;
        [SerializeField] private bool synchronizeRotation = true;
        [SerializeField] private bool synchronizeScale = false;

        protected Vector3 targetPosition;
        protected Quaternion targetRotation;
        protected Vector3 targetScale;

        public float lerpSpeed = 15;

        public bool TransformSynchronizationInitialized { get; private set; } = false;

        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (synchronizePosition)
                {
                    stream.SendNext(transform.position);
                }
                if (synchronizeRotation)
                {
                    stream.SendNext(transform.rotation);
                }
                if (synchronizeScale)
                {
                    stream.SendNext(transform.localScale);
                }
            }
            else
            {
                if (synchronizePosition)
                {
                    targetPosition = (Vector3)(stream.ReceiveNext());
                }
                if (synchronizeRotation)
                {
                    targetRotation = (Quaternion)(stream.ReceiveNext());
                }
                if (synchronizeScale)
                {
                    targetScale = (Vector3)(stream.ReceiveNext());
                }
                TransformSynchronizationInitialized = true;
            }
        }

        protected virtual void Update()
        {
            if (TransformSynchronizationInitialized && photonView.Owner != PhotonNetwork.LocalPlayer)
            {
                // smoothly interpolate synchronized values
                if (synchronizePosition)
                {
                    transform.position = SmoothVector3(transform.position, targetPosition, lerpSpeed);
                }
                if (synchronizeRotation)
                {
                    transform.rotation = SmoothQuaternion(transform.rotation, targetRotation, lerpSpeed);
                }
                if (synchronizeScale)
                {
                    transform.localScale = SmoothVector3(transform.localScale, targetScale, lerpSpeed);
                }
            }
        }

        public static Vector3 SmoothVector3(Vector3 current, Vector3 target, float lerpSpeed)
        {
            return Vector3.Lerp(current, target, lerpSpeed * Time.deltaTime);
        }

        public static Quaternion SmoothQuaternion(Quaternion current, Quaternion target, float lerpSpeed)
        {
            return Quaternion.Lerp(current, target, lerpSpeed * Time.deltaTime);
        }

        public static float SmoothFloat(float current, float target, float lerpSpeed)
        {
            return Mathf.Lerp(current, target, lerpSpeed * Time.deltaTime);
        }
    }
}