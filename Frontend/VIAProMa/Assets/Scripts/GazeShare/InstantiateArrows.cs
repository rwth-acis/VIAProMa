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
    protected Quaternion targetRotation;
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

    public void Start()
    {
        sharing = true;
        sharingGlobal = true;
        hololensIcon = Resources.Load<Sprite>("hololens");
        htcViveIcon = Resources.Load<Sprite>("htcVivePro");
    }

    /// <summary>
    /// Sends position, rotation, device and text to users in room
    /// Recieves position, rotation, device and text from other users in room and stores them in the target variables
    /// </summary>
    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.Serialize(ref deviceUsed);
            stream.Serialize(ref textToShow);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();
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
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && getIsUsingVive() == false && sharing == true && sharingGlobal == true)
        {
            // Copy rotation of DefaultCursor
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition;
            transform.rotation = target.transform.rotation;
            deviceUsed = 1;
        }
        else if (getIsUsingVive() == true && sharing == true && sharingGlobal == true)
        {
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            transform.position = getHitPositionOfPointedObjectFinal();
            transform.rotation = getHitRotationOfPointedObjectFinal();
            deviceUsed = 2;
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
            transform.rotation = targetRotation;
        }
        else
        {
            transform.position = far;
            transform.rotation = rot;
        }
    }

    /// <summary>
    /// Checks for the input source type of the detected controllers
    /// </summary>
    /// <remarks>
    /// Change InputSourceType.Controller to InputSourceType.Hand for testing in unity editor
    /// In unity editor use gesture hand from input simulation service found in the mixed reality toolkit to test as controller of HTC Vive
    /// </remarks>
    /// <returns>True if user is using HTC Vive and False if using Hololens</returns>
    protected bool getIsUsingVive()
    {
        isUsingVive = false;
        foreach (IMixedRealityController controller in MixedRealityToolkit.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceType == InputSourceType.Controller)
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
                return "orange";
                break;

            case 2:
                return "pink";
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
                return new Color(1.0f, 0.5f, 0f, 0.5f);
                break;

            case 2:
                return new Color(1.0f, 0f, 1.0f, 0.5f);
                break;

            case 3:
                return new Color(0f, 0f, 0f, 0.5f);
                break;

            default: return Color.white;
        }
    }

    /// <summary>
    /// Gets the renderer of the gameobject to change color
    /// </summary>
    protected void setColorOfArrow()
    {
        GetComponent<Renderer>().material.color = getColorForUser(photonView.OwnerActorNr);
    }

    /// <summary>
    /// Finds all the gamobjects with custom tag "showArrow"
    /// These are the game objects where the arrow can be put on
    /// </summary>
    /// <returns> Array with all the game objects with the "showArrow" tag</returns>
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