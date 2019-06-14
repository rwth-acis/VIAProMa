using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviourPun, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.eulerAngles);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.eulerAngles = (Vector3)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
