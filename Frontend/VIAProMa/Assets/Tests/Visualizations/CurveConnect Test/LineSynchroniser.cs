using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LineSynchroniser : MonoBehaviour, IPunObservable
{
    public LineRenderer line;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(line.colorGradient.colorKeys[0].color.r);
            stream.SendNext(line.colorGradient.colorKeys[0].color.g);
            stream.SendNext(line.colorGradient.colorKeys[0].color.b);
            stream.SendNext(line.colorGradient.colorKeys[0].color.a);

            stream.SendNext(line.colorGradient.colorKeys[1].color.r);
            stream.SendNext(line.colorGradient.colorKeys[1].color.g);
            stream.SendNext(line.colorGradient.colorKeys[1].color.b);
            stream.SendNext(line.colorGradient.colorKeys[1].color.a);
            stream.SendNext(line.positionCount);
            Vector3[] positions = new Vector3[line.positionCount];
            line.GetPositions(positions);
            stream.SendNext(positions);
        }
        else
        {
            Color c1 = new Color();
            c1.r = (float)stream.ReceiveNext();
            c1.g = (float)stream.ReceiveNext();
            c1.b = (float)stream.ReceiveNext();
            c1.a = (float)stream.ReceiveNext();
            Color c2 = new Color();
            c2.r = (float)stream.ReceiveNext();
            c2.g = (float)stream.ReceiveNext();
            c2.b = (float)stream.ReceiveNext();
            c2.a = (float)stream.ReceiveNext();
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );

            line.colorGradient = gradient;
            line.positionCount = (int)stream.ReceiveNext();
            line.SetPositions((Vector3[])stream.ReceiveNext());
        }
    }

    public void Start()
    {
<<<<<<< HEAD
        //line = GetComponent<LineRenderer>();
=======
>>>>>>> 658b1df17350e8bee188f8120b22de6d37634bd5
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.widthMultiplier = 0.025f;
    }
}

public enum ColorEnum
{
    connecting,
    connected
}
