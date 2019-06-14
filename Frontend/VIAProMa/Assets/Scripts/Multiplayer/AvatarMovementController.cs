using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AvatarMovementController : MonoBehaviourPun, IPunObservable
{
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    public float lerpSpeed = 1f;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.eulerAngles);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = Quaternion.Euler((Vector3)stream.ReceiveNext());
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // set the position based on the camera
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
        }
    }
}
