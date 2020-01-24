using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;

public class ChangeObjectHandler : MonoBehaviourPun, IPunObservable
{

    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Vector3 aLittleLeftOpen = new Vector3(0.05445f, 0.17583f, 0f);
    protected Vector3 aLittleLeftClosed = new Vector3(0f, 0.17583f, 0.05445f);
    protected Vector3 scaleFactor = new Vector3(0.5f, 0.5f, 0.5f);
    protected Mesh otherArrow;
    protected Mesh circularArrow;
    protected Mesh[] meshArray = new Mesh[2];
    protected int counter;

    void Start()
    {
        otherArrow = Resources.Load<Mesh>("otherArrow");
        circularArrow = Resources.Load<Mesh>("CircularArrow");
        meshArray[0] = otherArrow;
        meshArray[1] = circularArrow;
        counter = 0;
        GameObject.Find("ChangeMeshLabel").GetComponent<TextMeshPro>().text = "Change Object";
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            if (GameObject.Find("Left").transform.rotation.y < -0.51)
            {
                transform.position = GameObject.Find("No Gaze Button").transform.position + aLittleLeftOpen;
            }
            else
            {
                transform.position = GameObject.Find("No Gaze Button").transform.position + aLittleLeftClosed;
            }
            transform.rotation = GameObject.Find("No Gaze Button").transform.rotation;
            transform.localScale = Vector3.Scale(GameObject.Find("No Gaze Button").transform.localScale, scaleFactor);
        }
        else
        {
            transform.position = far;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }

    protected GameObject[] getAllGameObjectsArrow()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("arrow");
        return arrayAll;
    }

    public void changeGameObject()
    {
        foreach (GameObject arrow in getAllGameObjectsArrow())
        {
            if (arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr == photonView.OwnerActorNr)
            {
                arrow.GetComponent<MeshFilter>().mesh = meshArray[counter % 2];
                counter = counter + 1;
            }
        }
    }
}