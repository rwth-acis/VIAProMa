﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;
using TMPro;

public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{
    protected Vector3 targetPosition;
    protected Vector3 up = new Vector3(0f, 0f, 0f);
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
    protected bool isUsingVive;
    [HideInInspector] public bool sharing;
    [HideInInspector] public bool sharingGlobal;
    protected string deviceUsed;
    protected string deviceUsedTarget;
    protected string textToShow;


    public void Start()
    {
        deviceUsed = "No Device";
        sharing = true;
        sharingGlobal = true;
        setTextOfGlobalGazingLabel();
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.Serialize(ref deviceUsed);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            stream.Serialize(ref deviceUsedTarget);
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
        //setOverlayTextLabel();
        GameObject.Find("Main Menu").GetComponent<MainMenu>().canvas.GetComponentInChildren<Text>().text = textToShow;
        textToShow = "";
    }

    protected void moveMyArrow()
    {
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && getIsUsingVive() == false && sharing == true && sharingGlobal == true)
        {
            // Copy rotation of DefaultCursor
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            //Quaternion newRotation = target.transform.rotation;
            //Vector3 newRotation = new Vector3(target.transform.eulerAngles.x - 90f, target.transform.eulerAngles.y - 180f, target.transform.eulerAngles.z); // works for horizontal
            //Vector3 newRotation = new Vector3(target.transform.eulerAngles.x - 90f, target.transform.eulerAngles.y - 180f, target.transform.eulerAngles.z -180); // works for vertical
            Vector3 newRotation = new Vector3(target.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            //transform.rotation = Quaternion.Inverse(newRotation);
            transform.eulerAngles = newRotation;
            //GetComponentInChildren<TextMeshPro>().text = "Hololens";
            deviceUsed = "Hololens";
            //textToShow = textToShow + photonView.Owner.NickName + " " + getStringOfColor(photonView.OwnerActorNr) + " " + deviceUsed + "\n";
            textToShow = "";
        }
        else if (getIsUsingVive() == true && sharing == true && sharingGlobal == true)
        {
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            Vector3 newRotation = new Vector3(target.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
            transform.position = getHitPositionOfPointedObjectFinal() + up;
            transform.rotation = getHitRotationOfPointedObjectFinal();
            deviceUsed = "HTC Vive";
            //textToShow = textToShow + photonView.Owner.NickName + " " + getStringOfColor(photonView.OwnerActorNr) + " " + deviceUsed + "\n";
            textToShow = "";
        }
        else
        {
            transform.position = far;
            transform.rotation = rot;
        }
    }

    protected void moveOtherArrows()
    {
        if (sharingGlobal == true)
        {
            transform.position = targetPosition;
            textToShow = textToShow + photonView.Owner.NickName + ": On " + deviceUsedTarget + " with " + getStringOfColor(photonView.OwnerActorNr) + " arrow" + "\n";
        }
        else
        {
            transform.position = far;
        }

    }

    /*protected void setOverlayTextLabel()
    {
        textToShow = "";
        if (GameObject.Find("Main Menu").GetComponent<MainMenu>().canvas.activeSelf == true)
        {
            foreach (GameObject arrow in getAllGameObjectsArrow())
            {
                textToShow = textToShow + getNameOfOwner(arrow) + " " + getStringOfColor(arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr) + " " + arrow.GetComponent<InstantiateArrows>().deviceUsed + "\n";
            }
            GameObject.Find("Main Menu").GetComponent<MainMenu>().canvas.GetComponentInChildren<Text>().text = textToShow;
        }
    }*/

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

    protected string getStringOfColor(int userID)
    {
        switch (userID)
        {
            case 1:
                return "red";
                break;

            case 2:
                return "green";
                break;

            case 3:
                return "black";
                break;

            default: return "white";
        }
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

    protected GameObject getGlobalGazingLabelObject()
    {
        GameObject globalGazinglabelObject = GameObject.Find("GlobalGazingLabel");
        return globalGazinglabelObject;
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

    protected Quaternion getHitRotationOfPointedObjectFinal()
    {
        Quaternion hitRotationResult = rot;
        foreach (GameObject controller in getAllGameObjectsWithArrowScriptTesting())
        {
            if (controller.GetComponent<ArrowControllerHandler>().pointerHitPosition != far)
            {
                hitRotationResult = controller.GetComponent<ArrowControllerHandler>().pointerHitRotation;
            }
        }
        return hitRotationResult;
    }

    public void setTextOfGlobalGazingLabel()
    {
        if (sharingGlobal == true)
        {
            getGlobalGazingLabelObject().GetComponent<TextMeshPro>().text = "Disable Gazing";
        }
        else
        {
            getGlobalGazingLabelObject().GetComponent<TextMeshPro>().text = "Enable Gazing";
        }
    }

    protected GameObject[] getAllGameObjectsArrow()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("arrow");
        return arrayAll;
    }

    protected string getNameOfOwner(GameObject arrow)
    {
        return arrow.GetComponent<InstantiateArrows>().photonView.Owner.NickName;
    }

    /*protected string getDeviceUsed(GameObject arrow)
    {
        string device = "No Device";
        if (arrow.GetComponent<InstantiateArrows>().getIsUsingVive() == true)
        {
            device = "HTC Vive";
        }
        else
        {
            device = "Hololens";
        }
        return device;
    }*/
}