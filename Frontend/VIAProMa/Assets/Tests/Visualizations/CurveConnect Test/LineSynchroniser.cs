using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LineSynchroniser : MonoBehaviour, IPunObservable
{
    LineRenderer line;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(line.material);
            stream.SendNext(line.widthMultiplier);
            stream.SendNext(line.colorGradient);
            stream.SendNext(line.positionCount);
            Vector3[] positions = new Vector3[line.positionCount];
            line.GetPositions(positions);
            stream.SendNext(positions);
        }
        else
        {
            line.material = (Material)stream.ReceiveNext();
            line.widthMultiplier = (float)stream.ReceiveNext();
            line.colorGradient = (Gradient)stream.ReceiveNext();
            line.positionCount = (int)stream.ReceiveNext();
            line.SetPositions((Vector3[])stream.ReceiveNext());
        }
    }

    public void Start()
    {
        line = GetComponent<LineRenderer>();
    }
}
