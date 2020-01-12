using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;


public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{

    protected Vector3 targetPosition;
    protected Vector3 up = new Vector3(0f, 0.1f, 0f);
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    public float lerpSpeed = 1f;
    protected Quaternion rot = Quaternion.Euler(0, 0, -90);
    //private bool isSharing = true;
    //Material mat;

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if we are writing to the stream => we are the local player and want to transmit our position and rotations
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else // we are reading the network stream, i.e. we receive a remote player position and rotation
        {
            targetPosition = (Vector3)stream.ReceiveNext();
        }
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            transform.rotation = rot;
                //Vector3 angle = new Vector3(gameObject.transform.eulerAngles.x, giveGaze().GazeDirection.x * 90, gameObject.transform.eulerAngles.z);
                //gameObject.transform.eulerAngles = angle;
                //txt.text = giveGaze().GazeTarget.name;
                //isSharing = true;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed * Time.deltaTime);
        }
    }
}
