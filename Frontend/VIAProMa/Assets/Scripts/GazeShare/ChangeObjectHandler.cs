using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;

public class ChangeObjectHandler : MonoBehaviourPun, IPunObservable
{

    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Vector3 aLittleLeftOpen = new Vector3(0.05445f, 0f, 0f);
    protected Vector3 aLittleLeftClosed = new Vector3(0f, 0f, 0.05445f);
    protected Vector3 scaleFactor = new Vector3(0.5f, 0.5f, 0.5f);
    protected Mesh circularArrow;
    protected Mesh conicArrow;
    protected Mesh monkeyArrow;
    protected Mesh sphericArrow;
    protected Mesh[] meshArray = new Mesh[4];
    protected int counter;
    protected int targetCounter;

    void Start()
    {
        circularArrow = Resources.Load<Mesh>("CircularArrow");
        conicArrow = Resources.Load<Mesh>("ConicArrow");
        monkeyArrow = Resources.Load<Mesh>("MonkeyArrow");
        sphericArrow = Resources.Load<Mesh>("SphericArrow");
        meshArray[0] = circularArrow;
        meshArray[1] = conicArrow;
        meshArray[2] = monkeyArrow;
        meshArray[3] = sphericArrow;
        counter = 0;
        GameObject.Find("ChangeMeshLabel").GetComponent<TextMeshPro>().text = "Change Object";
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            setCorrectTransform();
        }
        else
        {
            foreach (GameObject arrow in getAllGameObjectsArrow())
            {
                if (arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr == photonView.OwnerActorNr)
                {
                    arrow.GetComponent<MeshFilter>().mesh = meshArray[(targetCounter + 1) % 4] ;
                }
            }
            transform.position = far;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.Serialize(ref counter);
        }
        else
        {
            stream.Serialize(ref targetCounter);
        }
    }

    protected GameObject[] getAllGameObjectsArrow()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("arrow");
        return arrayAll;
    }

    protected void setCorrectTransform()
    {
        if (GameObject.Find("Left") != null)
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
    }

    public void changeGameObject()
    {
        foreach (GameObject arrow in getAllGameObjectsArrow())
        {
            if (arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr == photonView.OwnerActorNr)
            {
                arrow.GetComponent<MeshFilter>().mesh = meshArray[counter % 4];
                counter = counter + 1;
            }
        }
    }
}