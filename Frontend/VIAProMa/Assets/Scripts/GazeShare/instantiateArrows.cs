using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class instantiateArrows : MonoBehaviourPun, IPunObservable
{

    protected Vector3 targetPosition;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        }
        else
        {
            targetPosition = (Vector3)(stream.ReceiveNext());
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("arrow2", new Vector3(0, 0.5f, 1.45f), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!(photonView.IsMine))
        {
            transform.position = targetPosition;
        }
    }
}
