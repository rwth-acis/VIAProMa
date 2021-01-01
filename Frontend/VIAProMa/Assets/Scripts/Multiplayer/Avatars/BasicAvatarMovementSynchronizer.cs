using Photon.Pun;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars
{
    /// <summary>
    /// Monitors the user's movement and synchronizes it with remote users
    /// Also handles incoming synchronization messages about the remote user movements
    /// </summary>
    public class BasicAvatarMovementSynchronizer : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] protected Transform avatarLeftHand;
        [SerializeField] protected Transform avatarRightHand;

        protected Transform playerLeftHand, playerRightHand;
        protected float timeSinceLastSearch;

        protected Vector3 targetPosition;
        protected Quaternion targetRotation;
        protected Vector3 leftHandTargetPosition, rightHandTargetPosition;
        protected Quaternion leftHandTargetRotation, rightHandTargetRotation;

        protected Transform mainCamera;

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

        protected virtual void Awake()
        {
            mainCamera = Camera.main.transform; // cache it because it because Camera.main costs performance
        }

        /// <summary>
        /// Called by Photon if a network message is serialized or deserialized
        /// The function is only called if the GameObject also has a PhotonView component where this component is registered
        /// </summary>
        /// <param name="stream">The network stream</param>
        /// <param name="info">Information about the network message</param>
        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // if we are writing to the stream => we are the local player and want to transmit our position and rotations
            if (stream.IsWriting)
            {
                if (timeSinceLastSearch > handSearchInterval)
                {
                    SearchHands();
                }

                // send the position and euler rotation of the camera
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                // send the position and rotation of the left hand
                Vector3 leftHandPosition = EncodeHandPosition(playerLeftHand);
                stream.SendNext(leftHandPosition);
                Quaternion leftHandRotation = EncodeHandRotation(playerLeftHand);
                stream.SendNext(leftHandRotation);
                // send the position and rotation of the right hand
                Vector3 rightHandPosition = EncodeHandPosition(playerRightHand);
                stream.SendNext(rightHandPosition);
                Quaternion rightHandRotation = EncodeHandRotation(playerRightHand);
                stream.SendNext(rightHandRotation);
            }
            else // we are reading the network stream, i.e. we receive a remote player position and rotation
            {
                // get the position and rotation of the remote player's camera
                targetPosition = (Vector3)stream.ReceiveNext();
                targetRotation = (Quaternion)stream.ReceiveNext();
                // get the position of the left hand
                leftHandTargetPosition = (Vector3)stream.ReceiveNext();
                leftHandTargetRotation = (Quaternion)stream.ReceiveNext();
                // get the position of the right hand
                rightHandTargetPosition = (Vector3)stream.ReceiveNext();
                rightHandTargetRotation = (Quaternion)stream.ReceiveNext();
            }
        }

        /// <summary>
        /// If we are the local player, it sets the avatar's position and rotations to match the camera and hands
        /// This way, it suffices to synchronize the position and rotation of the avatar
        /// If this component is controlled by a remoted player,  smoothly transition between the received positions and rotations
        /// </summary>
        protected virtual void Update()
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
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
                MoveAvatarHand(avatarLeftHand, leftHandTargetPosition, leftHandTargetRotation);
                MoveAvatarHand(avatarRightHand, rightHandTargetPosition, rightHandTargetRotation);
            }
        }

        /// <summary>
        /// Tries to find the hands
        /// </summary>
        protected void SearchHands()
        {
            if (playerLeftHand == null)
            {
                playerLeftHand = mainCamera.transform.parent.Find("Left_DefaultControllerPointer(Clone)");
            }
            if (playerRightHand == null)
            {
                playerRightHand = mainCamera.transform.parent.Find("Right_DefaultControllerPointer(Clone)");
            }
            timeSinceLastSearch = 0f;
        }

        /// <summary>
        /// Encodes the hand position for networking
        /// In order to keep the network messages small, a non-tracked hand is represented by the 0-vector (it is very unlikely that the user will position the hand exactly in this position)
        /// </summary>
        /// <param name="hand">The hand to encode</param>
        /// <returns>The position of the hand if it is tracked or the 0-vector otherwise</returns>
        protected Vector3 EncodeHandPosition(Transform hand)
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

        protected Quaternion EncodeHandRotation(Transform hand)
        {
            if (hand == null)
            {
                return Quaternion.identity;
            }
            else
            {
                return hand.rotation;
            }
        }

        /// <summary>
        /// Decodes the handTargetPosition which was received over the network:
        /// If handTargetPosition is the 0-vector, it means that the hand is not tracked
        /// </summary>
        /// <param name="handAvatar">The avatar part which represents the hand</param>
        /// <param name="handTargetPosition">The received position of the hand</param>
        protected void MoveAvatarHand(Transform handAvatar, Vector3 handTargetPosition, Quaternion handTargetRotation)
        {
            if (handTargetPosition == Vector3.zero && handTargetRotation == Quaternion.identity)
            {
                handAvatar.gameObject.SetActive(false);
            }
            else
            {
                handAvatar.gameObject.SetActive(true);
                handAvatar.transform.position = Vector3.Lerp(handAvatar.position, handTargetPosition, lerpSpeed * Time.deltaTime);
                handAvatar.transform.rotation = Quaternion.Slerp(handAvatar.rotation, handTargetRotation, lerpSpeed * Time.deltaTime);
            }
        }
    }
}