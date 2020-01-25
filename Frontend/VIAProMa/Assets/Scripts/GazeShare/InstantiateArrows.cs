using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Monitors the user's arrow and synchronizes it with remote users
/// Handles incoming synchronization messages about the remote users arrows
/// </summary>
public class InstantiateArrows : MonoBehaviourPun, IPunObservable
{
    protected Vector3 targetPosition;
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
    protected bool isUsingVive;
    [HideInInspector] public bool sharing;
    [HideInInspector] public bool sharingGlobal;
    protected int deviceUsed;
    protected int deviceUsedTarget;
    protected string textToShow;
    protected string targetTextToShow;
    protected Sprite hololensIcon;
    protected Sprite htcViveIcon;

    //public float lerpSpeed = 15f;

    public void Start()
    {
        //deviceUsed = "No Device";
        sharing = true;
        sharingGlobal = true;
        setTextOfGlobalGazingLabel();
        hololensIcon = Resources.Load<Sprite>("hololens");
        htcViveIcon = Resources.Load<Sprite>("htcVivePro");
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.Serialize(ref deviceUsed);
            stream.Serialize(ref textToShow);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            stream.Serialize(ref deviceUsedTarget);
            stream.Serialize(ref targetTextToShow);
        }
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            moveMyArrow();
            setColorOfArrow();
            textToShow = photonView.Owner.NickName;
            gameObject.GetComponentInChildren<TextMeshPro>().text = textToShow;
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = getIconForDevice(deviceUsed);
        }
        else
        {
            moveOtherArrows();
            setColorOfArrow();
            gameObject.GetComponentInChildren<TextMeshPro>().text = targetTextToShow;
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = getIconForDevice(deviceUsedTarget);
        }
    }

    /// <summary>
    /// Checks if user is using Hololens or HTC Vive
    /// and also checks if sharing and global sharing are enabled to
    /// set the correct position and rotation of arrow
    /// </summary>
    protected void moveMyArrow()
    {
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && getIsUsingVive() == false && sharing == true && sharingGlobal == true && isHololensLookingAtMesh() == false)
        {
            // Copy rotation of DefaultCursor
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            //Quaternion newRotation = target.transform.rotation;
            //Vector3 newRotation = new Vector3(target.transform.eulerAngles.x - 90f, target.transform.eulerAngles.y - 180f, target.transform.eulerAngles.z); // works for horizontal
            //Vector3 newRotation = new Vector3(target.transform.eulerAngles.x - 90f, target.transform.eulerAngles.y - 180f, target.transform.eulerAngles.z -180); // works for vertical
            Vector3 newRotation = new Vector3(target.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition;
            //transform.rotation = Quaternion.Inverse(newRotation);
            transform.eulerAngles = newRotation;
            //GetComponentInChildren<TextMeshPro>().text = "Hololens";
            deviceUsed = 1;
            //textToShow = textToShow + photonView.Owner.NickName + " " + getStringOfColor(photonView.OwnerActorNr) + " " + deviceUsed + "\n";
        }
        else if (getIsUsingVive() == true && sharing == true && sharingGlobal == true)
        {
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            Vector3 newRotation = new Vector3(target.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
            transform.position = getHitPositionOfPointedObjectFinal();
            transform.rotation = getHitRotationOfPointedObjectFinal();
            deviceUsed = 2;
            //textToShow = textToShow + photonView.Owner.NickName + " " + getStringOfColor(photonView.OwnerActorNr) + " " + deviceUsed + "\n";
        }
        else
        {
            transform.position = far;
            transform.rotation = rot;
        }
    }

    /// <summary>
    /// Checks if global sharing is enabled to set the 
    /// correct position and rotation of the remoted player
    /// that is controlling this component
    /// </summary>
    protected void moveOtherArrows()
    {
        if (sharingGlobal == true)
        {
            //transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            transform.position = targetPosition;
        }
        else
        {
            transform.position = far;
        }
    }

    protected bool isHololensLookingAtMesh()
    {
        bool isLooking = false;
        if (MixedRealityToolkit.SpatialAwarenessSystem.SpatialAwarenessObjectParent == MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget)
        {
            isLooking = true;
        }
        foreach (Transform child in MixedRealityToolkit.SpatialAwarenessSystem.SpatialAwarenessObjectParent.transform)
        {
            if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget == child.gameObject)
            {
                isLooking = true;
            }
        }
        return isLooking;
    }

    /// <summary>
    /// Checks for the input source type of the detected controllers
    /// </summary>
    /// <returns>True if user is using HTC Vive and False if using Hololens</returns>
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

    /// <summary>
    /// Depending on the owner actor number this method gives the color of the arrow in text form
    /// </summary>
    /// <param name="userID">A owner actor number of a photon view</param>
    /// <returns>The color of the arrow in text form</returns>
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

    /// <summary>
    /// Depending on the owner actor number this method gives the color of the arrow
    /// </summary>
    /// <param name="userID">A owner actor number of a photon view</param>
    /// <returns>The color of the arrow</returns>
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

    /// <summary>
    /// Checks all objects that can be hit by a controller(pointer)
    /// by finding the objects having the correct tag(show arrow) in the scene
    /// </summary>
    /// <returns>The hit position of the controller(pointer) on the object</returns>
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

    /// <returns>The hit rotation of the controller(pointer) on the object</returns>
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

    protected string getNameOfOwner(GameObject arrow)
    {
        return arrow.GetComponent<InstantiateArrows>().photonView.Owner.NickName;
    }

    protected Sprite getIconForDevice(int device)
    {
        if (device == 1)
        {
            return hololensIcon;
        }
        else
        {
            return htcViveIcon;
        }
    }
}