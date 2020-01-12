using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;

public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{
    protected ArrowControllerHandler arrowScriptMenu;
    protected ArrowControllerHandler arrowScriptLeft;
    protected ArrowControllerHandler arrowScriptRight;
    protected Vector3 targetPosition;
    protected Vector3 up = new Vector3(0f, 0.1f, 0f);
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    public float lerpSpeed = 1f;
    protected Quaternion rot = Quaternion.Euler(0, 0, -90);
    protected bool isUsingVive;

    public void Start()
    {
        GameObject mainMenu = GameObject.Find("Main Menu");
        GameObject left = GameObject.Find("leftTestCube");
        GameObject right = GameObject.Find("rightTestCube");
        arrowScriptMenu = mainMenu.GetComponent<ArrowControllerHandler>();
        arrowScriptLeft = left.GetComponent<ArrowControllerHandler>();
        arrowScriptRight = right.GetComponent<ArrowControllerHandler>();
    }

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
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && getIsUsingVive() == false/*&& isSharing == true*/)
        {
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            transform.rotation = rot;
            Debug.Log("1");
        }
        else if (getIsUsingVive() == true)
        {
            Debug.Log("2");
            /*if(arrowScriptLeft.pointerHitPosition != far)
            {
                transform.position = arrowScriptLeft.pointerHitPosition;
            }
            else if(arrowScriptRight.pointerHitPosition != far)
            {
                transform.position = arrowScriptRight.pointerHitPosition;
            }
            else if (arrowScriptMenu.pointerHitPosition != far)
            {
                transform.position = arrowScriptMenu.pointerHitPosition;
            }*/
            transform.rotation = rot;
            transform.position = arrowScriptMenu.pointerHitPosition;
        }
        else
        {
            transform.position = far;
            Debug.Log("3");
        }
    }

    protected bool getIsUsingVive()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            //Debug.Log("Controller is : " + controller.InputSource.SourceType);
            if (controller.InputSource.SourceType == InputSourceType.Controller)
            {
                isUsingVive = true;
            }
        }
        return isUsingVive;
    }

    protected void moveOtherArrows()
    {
        transform.position = targetPosition;
        transform.rotation = rot;
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
    /*
    protected bool getIsUsingVive()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            //Debug.Log("Controller is : " + controller.InputSource.SourceType);
            if (controller.InputSource.SourceType == InputSourceType.Hand)
            {
                isUsingVive = true;
            }
        }
        return isUsingVive;
    }
    
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        Debug.Log("5");
        Debug.Log("isUsing :" + getIsUsingVive());
        if(getIsUsingVive() == true)
        {
            var result = eventData.Pointer.Result;
            Debug.Log("result :" + result);
            if (result != null)
            {
                transform.position = result.Details.Point;
            }
            else { transform.position = far; }
        }
    }*/
}