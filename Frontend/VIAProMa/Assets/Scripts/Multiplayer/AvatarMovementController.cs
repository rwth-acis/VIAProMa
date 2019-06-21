using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AvatarMovementController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Transform avatarLeftHand;
    [SerializeField] private Transform avatarRightHand;

    private Transform playerLeftHand, playerRightHand;
    private float timeSinceLastSearch;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 leftHandTargetPosition, rightHandTargetPosition;

    public float lerpSpeed = 1f;
    public float handSearchInterval = 1f;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("Sending camera data");
            if (timeSinceLastSearch > handSearchInterval)
            {
                CheckHandPositions();
            }

            // send the position and euler rotation of the camera
            stream.SendNext(transform.position);
            stream.SendNext(transform.eulerAngles);
            // send the position of the left hand
            Vector3 leftHandPosition = EncodeHandPosition(playerLeftHand);
            stream.SendNext(leftHandPosition);
            // send the position of the right hand
            Vector3 rightHandPosition = EncodeHandPosition(playerRightHand);
            stream.SendNext(rightHandPosition);
        }
        else
        {
            // get the position and rotation of the remote player's camera
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = Quaternion.Euler((Vector3)stream.ReceiveNext());
            // get the position of the left hand
            leftHandTargetPosition = (Vector3)stream.ReceiveNext();
            // get the position of the right hand
            rightHandTargetPosition = (Vector3)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // set the position based on the camera
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
            timeSinceLastSearch += Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
            MoveAvatarHand(avatarLeftHand, leftHandTargetPosition);
            MoveAvatarHand(avatarRightHand, rightHandTargetPosition);
        }
    }

    private void CheckHandPositions()
    {
        if (playerLeftHand == null)
        {
            playerLeftHand = Camera.main.transform.parent.Find("Left_DefaultControllerPointer(Clone)");
        }
        if (playerRightHand == null)
        {
            playerRightHand = Camera.main.transform.parent.Find("Right_DefaultControllerPointer(Clone)");
        }
        timeSinceLastSearch = 0f;
    }

    private Vector3 EncodeHandPosition(Transform hand)
    {
        if (hand == null)
        {
            return Vector3.zero;
        }
        else
        {
            return hand.position;
        }
    }

    private void MoveAvatarHand(Transform handAvatar, Vector3 handTargetPosition)
    {
        if (handTargetPosition == Vector3.zero)
        {
            handAvatar.gameObject.SetActive(false);
        }
        else
        {
            handAvatar.gameObject.SetActive(true);
            transform.position = Vector3.Lerp(handAvatar.position, handTargetPosition, lerpSpeed * Time.deltaTime);
        }
    }
}
