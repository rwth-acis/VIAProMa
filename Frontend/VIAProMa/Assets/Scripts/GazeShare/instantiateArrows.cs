using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;

public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{
    protected Vector3 targetPosition;
    protected Vector3 up = new Vector3(0f, 0.025f, 0f);
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Quaternion rot = Quaternion.Euler(0, 0, -90);
    protected bool isUsingVive;
    protected int clickableObjectCount = 0;

    /*public void Start()
    {

    }*/

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            moveMyArrow();
            setColorOfArrow();
        }
        else
        {
            moveOtherArrows();
            setColorOfArrow();
        }
    }

    protected void moveMyArrow()
    {
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && getIsUsingVive() == false)
        {
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            transform.rotation = rot;
        }
        else if (getIsUsingVive() == true)
        {
            transform.rotation = rot;
            transform.position = getHitPositionOfPointedObjectFinal();
        }
        else
        {
            transform.position = far;
        }
    }

    protected void moveOtherArrows()
    {
        transform.position = targetPosition;
        transform.rotation = rot;
    }

    protected bool getIsUsingVive()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceType == InputSourceType.Hand)
            {
                isUsingVive = true;
            }
        }
        return isUsingVive;
    }

    protected Color getColorForUser(int userID)
    {
        switch (userID)
        {
            case 1:
                return Color.red;
                break;

            case 2:
                return Color.green;
                break;

            case 3:
                return Color.black;
                break;

            default: return Color.white;
        }
    }

    protected void setColorOfArrow()
    {
        GetComponent<Renderer>().material.color = getColorForUser(photonView.OwnerActorNr);
    }

    protected GameObject[] getAllGameObjectsWithArrowScriptTesting()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("showArrow");
        return arrayAll;
    }

    protected Vector3 getHitPositionOfPointedObjectFinal()
    {
        Vector3 hitPositionResult = far;
        foreach (GameObject controller in getAllGameObjectsWithArrowScriptTesting())
        {
            if (controller.GetComponent<ArrowControllerHandler>().pointerHitPosition != far)
            {
                hitPositionResult = controller.GetComponent<ArrowControllerHandler>().pointerHitPosition;
            }
        }
        return hitPositionResult;
    }
}