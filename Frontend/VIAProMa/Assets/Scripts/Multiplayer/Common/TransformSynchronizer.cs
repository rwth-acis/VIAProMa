using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSynchronizer : MonoBehaviourPun, IPunObservable
{
    protected Vector3 targetPosition;
    protected Quaternion targetRotation;

    public float lerpSpeed = 15;

    private bool initialized = false;

    public static void PutPositionRotation(Transform targetTransform, ref PhotonStream stream)
    {
        stream.SendNext(targetTransform.position);
        stream.SendNext(targetTransform.rotation);
    }

    public static void GetPositionRotation(ref PhotonStream stream, out Vector3 position, out Quaternion rotation)
    {
        position = (Vector3)(stream.ReceiveNext());
        rotation = (Quaternion)(stream.ReceiveNext());
    }

    public static void SmoothPosition(Transform targetTransform, Vector3 targetPosition, float lerpSpeed)
    {
        targetTransform.position = Vector3.Lerp(targetTransform.position, targetPosition, lerpSpeed * Time.deltaTime);
    }

    public static void SmoothRotation(Transform targetTransform, Quaternion targetRotation, float lerpSpeed)
    {
        targetTransform.rotation = Quaternion.Lerp(targetTransform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            PutPositionRotation(transform, ref stream);
        }
        else
        {
            GetPositionRotation(ref stream, out targetPosition, out targetRotation);
            initialized = true;
        }
    }

    protected virtual void Update()
    {
        if (initialized)
        {
            SmoothPosition(transform, targetPosition, lerpSpeed);
            SmoothRotation(transform, targetRotation, lerpSpeed);
        }
    }
}
