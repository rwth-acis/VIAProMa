using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;


public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{

    protected Vector3 targetPosition;
    private Vector3 up = new Vector3(0f, 0.1f, 0f);
    private Vector3 far = new Vector3(0f, -10f, 0f);
    //private bool isSharing = true;
    //Material mat;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
        }
    }

    public IMixedRealityGazeProvider giveGaze()
    {
        IMixedRealityGazeProvider currentGaze = MixedRealityToolkit.InputSystem.GazeProvider;
        return currentGaze;
    }

    // Start is called before the first frame update
    void Start()
    {
        // PhotonNetwork.Instantiate("arrow2", new Vector3(0, 0.5f, 1.45f), Quaternion.identity, 0);
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
                 Vector3 currentHitPosition = giveGaze().HitPosition;
                transform.position = currentHitPosition + up;
                //Vector3 angle = new Vector3(gameObject.transform.eulerAngles.x, giveGaze().GazeDirection.x * 90, gameObject.transform.eulerAngles.z);
                //gameObject.transform.eulerAngles = angle;
                //txt.text = giveGaze().GazeTarget.name;
                //isSharing = true;
        }
        else
        {
            transform.position = targetPosition;
        }
    }
}
