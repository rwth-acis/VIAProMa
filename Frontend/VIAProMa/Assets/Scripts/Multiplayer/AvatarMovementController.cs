using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Monitors the user's movement and synchronizes it with remote users
/// Also handles incoming synchronization messages about the remote user movements
/// </summary>
public class AvatarMovementController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Transform avatarLeftHand;
    [SerializeField] private Transform avatarRightHand;

    private Transform playerLeftHand, playerRightHand;
    private float timeSinceLastSearch;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 leftHandTargetPosition, rightHandTargetPosition;

    /// <summary>
    /// Specifies how quickly the avatar moves from one position to the next
    /// This value is required in order to get smooth transitions
    /// </summary>
    public float lerpSpeed = 1f;
    /// <summary>
    /// If the hands are not currently tracked, the hand will be searched every few seconds
    /// This value speciies how often the hands are searched
    /// </summary>
    public float handSearchInterval = 1f;

    /// <summary>
    /// Called by Photon if a network message is serialized or deserialized
    /// The function is only called if the GameObject also has a PhotonView component where this component is registered
    /// </summary>
    /// <param name="stream">The network stream</param>
    /// <param name="info">Information about the network message</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if we are writing to the stream => we are the local player and want to transmit our position and rotations
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
        else // we are reading the network stream, i.e. we receive a remote player position and rotation
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

    /// <summary>
    /// If we are the local player, it sets the avatar's position and rotations to match the camera and hands
    /// This way, it suffices to synchronize the position and rotation of the avatar
    /// If this component is controlled by a remoted player,  smoothly transition between the received positions and rotations
    /// </summary>
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

    /// <summary>
    /// Tries to find the hands
    /// </summary>
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

    /// <summary>
    /// Encodes the hand position for networking
    /// In order to keep the network messages small, a non-tracked hand is represented by the 0-vector (it is very unlikely that the user will position the hand exactly in this position)
    /// </summary>
    /// <param name="hand">The hand to encode</param>
    /// <returns>The position of the hand if it is tracked or the 0-vector otherwise</returns>
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

    /// <summary>
    /// Decodes the handTargetPosition which was received over the network:
    /// If handTargetPosition is the 0-vector, it means that the hand is not tracked
    /// </summary>
    /// <param name="handAvatar">The avatar part which represents the hand</param>
    /// <param name="handTargetPosition">The received position of the hand</param>
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
