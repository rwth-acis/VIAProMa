using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;

/// <summary>
/// Monitors the user's change object button
/// </summary>
public class ChangeObjectHandler : MonoBehaviourPun, IPunObservable
{

    private Vector3 far = new Vector3(0f, -10f, 0f);
    private Vector3 aLittleLeftOpen = new Vector3(0.05445f, 0f, 0f);
    private Vector3 aLittleLeftClosed = new Vector3(0f, 0f, 0.05445f);
    private Vector3 scaleFactor = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Mesh cylindricalArrowMesh;
    [SerializeField] private Mesh conicArrowMesh;
    [SerializeField] private Mesh monkeyArrowMesh;
    [SerializeField] private Mesh sphericalArrowMesh;
    private Mesh[] meshArray = new Mesh[4];
    private int counter;
    private int targetCounter;


    /// <summary>
    /// Saves the used meshes in refrences
    /// Fills the mesh array
    /// </summary>
    void Start()
    {
        meshArray[0] = cylindricalArrowMesh; 
        meshArray[1] = conicArrowMesh; 
        meshArray[2] = monkeyArrowMesh; 
        meshArray[3] = sphericalArrowMesh; 
        counter = 0;
        targetCounter = 0;
        GameObject.Find("ChangeMeshLabel").GetComponent<TextMeshPro>().text = "Change Object";
    }

    /// <summary>
    /// Sets correct transform of button and applies the correct mesh to the other users arrows
    /// </summary>
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
                    arrow.GetComponent<MeshFilter>().mesh = meshArray[targetCounter % 4] ;
                }
            }
            transform.position = far;
        }
    }

    /// <summary>
    /// Sends counter to other users that allows the correct mesh to be set on the arrow
    /// Recieves counter and saves it to target variable to set the other users correct mesh of the arrow
    /// </summary>
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

    /// <summary>
    /// Checks if left side of the main menu cube is opened or closed
    /// by looking at its rotation and sets the change object button position
    /// correctly in either case, also sets the rotation and the right scale
    /// </summary>
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

    /// <summary>
    /// Sets the mesh of the arrow to the next mesh in the mesh array
    /// </summary>
    public void changeGameObject()
    {
        foreach (GameObject arrow in getAllGameObjectsArrow())
        {
            if (arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr == photonView.OwnerActorNr)
            {
                counter = counter + 1;
                arrow.GetComponent<MeshFilter>().mesh = meshArray[counter % 4];
            }
        }
    }
}