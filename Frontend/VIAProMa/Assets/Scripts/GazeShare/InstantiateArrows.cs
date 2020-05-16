using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public static class StaticGaze
{
    [HideInInspector] public static bool sharing { get; private set; }

    /// <summary>
    /// Checks for the input source type of the detected controllers
    /// </summary>
    /// <remarks>
    /// Change InputSourceType.Controller to InputSourceType.Hand for testing in unity editor
    /// In unity editor use gesture hand from input simulation service found in the mixed reality toolkit to test as controller of HTC Vive
    /// </remarks>
    /// <returns>True if user is using HTC Vive and False if using Hololens</returns>
    public static bool GetIsUsingVive()
    {
        bool isUsingVive = false;
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
    /// Finds all the gamobjects with custom tag "arrow"
    /// These are the pointer game objects
    /// </summary>
    /// <returns> Array with all the game objects with the "showArrow" tag</returns>
    public static GameObject[] GetAllArrowGameObjects()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("arrow");
        return arrayAll;
    }

    /// <summary>
    /// Finds all the gamobjects with custom tag "showArrow"
    /// These are the game objects where the arrow can be put on
    /// </summary>
    /// <returns> Array with all the game objects with the "showArrow" tag</returns>
    public static GameObject[] GetAllGameObjectsWithArrow()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("showArrow");
        return arrayAll;
    }

    /// <summary>
    /// Finds all the gamobjects with custom tag "avatar"
    /// These are the avatar game objects
    /// </summary>
    /// <returns> Array with all the game objects with the "showArrow" tag</returns>
    public static GameObject[] GetAllAvatarGameObjects()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("avatar");
        return arrayAll;
    }

    public static void InstantiateSharing()
    {
        sharing = true;
    }

    public static void ToggleSharing()
    {
        sharing = !sharing;
    }
}

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
    protected int deviceUsed;
    protected int deviceUsedTarget;
    protected string textToShow;
    protected string targetTextToShow;
    protected Sprite hololensIcon;
    protected Sprite htcViveIcon;
    [HideInInspector] public bool sharingGlobal { get; private set; }


    public void Start()
    {
        StaticGaze.InstantiateSharing();
        InstantiateSharingGlobal();
        hololensIcon = Resources.Load<Sprite>("hololens");
        htcViveIcon = Resources.Load<Sprite>("htcVivePro");
    }

    public void InstantiateSharingGlobal()
    {
        sharingGlobal = true;
    }

    public void ToggleSharingGlobal()
    {
        sharingGlobal = !sharingGlobal;
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
            MoveMyArrow();
            SetColorOfArrow();
            textToShow = photonView.Owner.NickName;
            gameObject.GetComponentInChildren<TextMeshPro>().text = textToShow;
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GetIconForDevice(deviceUsed);
        }
        else
        {
            MoveOtherArrows();
            SetColorOfArrow();
            gameObject.GetComponentInChildren<TextMeshPro>().text = targetTextToShow;
            gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GetIconForDevice(deviceUsedTarget);
        }
    }

    /// <summary>
    /// Checks if user is using Hololens or HTC Vive
    /// and also checks if sharing and global sharing are enabled to
    /// set the correct position and rotation of arrow
    /// </summary>
    protected void MoveMyArrow()
    {
        if (MixedRealityToolkit.InputSystem.GazeProvider.GazeTarget && StaticGaze.GetIsUsingVive() == false && StaticGaze.sharing == true && sharingGlobal == true)
        {
            // Copy rotation of DefaultCursor
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition;
            transform.rotation = target.transform.rotation;
            deviceUsed = 1;
        }
        else if (StaticGaze.GetIsUsingVive() == true && StaticGaze.sharing == true && sharingGlobal == true)
        {
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            transform.position = GetHitPositionOfPointedObjectFinal();
            transform.rotation = GetHitRotationOfPointedObjectFinal();
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
    protected void MoveOtherArrows()
    {
        if (sharingGlobal == true)
        {
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
    /// Depending on the owner actor number this method gives the color of the arrow
    /// </summary>
    /// <param name="userID">A owner actor number of a photon view</param>
    /// <returns>The color of the arrow</returns>
    protected Color GetColorForUser(int userID)
    {
        var hue = Random.Range(0f, 1f);
        return Color.HSVToRGB(hue, 1f, 1f);
        /*switch (userID)
        {
            case 1:
                return new Color(1.0f, 0.5f, 0f, 0.5f);
            case 2:
                return new Color(1.0f, 0f, 1.0f, 0.5f);
            case 3:
                return new Color(0f, 0f, 0f, 0.5f);
            default:
                return Color.white;
        }*/
    }

    /// <summary>
    /// Gets the renderer of the gameobject to change color
    /// </summary>
    protected void SetColorOfArrow()
    {
        GetComponent<Renderer>().material.color = GetColorForUser(photonView.OwnerActorNr);
    }

    protected GameObject GetGlobalGazingLabelObject()
    {
        GameObject globalGazinglabelObject = GameObject.Find("GlobalGazingLabel");
        return globalGazinglabelObject;
    }

    /// <summary>
    /// Checks all objects that can be hit by a controller(pointer)
    /// by finding the objects having the correct tag(show arrow) in the scene
    /// </summary>
    /// <returns>The hit position of the controller(pointer) on the object</returns>
    protected Vector3 GetHitPositionOfPointedObjectFinal()
    {
        Vector3 hitPositionResult = far;
        foreach (GameObject controller in StaticGaze.GetAllGameObjectsWithArrow())
        {
            if (controller.GetComponent<ArrowControllerHandler>().pointerHitPosition != far)
            {
                hitPositionResult = controller.GetComponent<ArrowControllerHandler>().pointerHitPosition;
            }
        }
        return hitPositionResult;
    }

    /// <returns>The hit rotation of the controller(pointer) on the object</returns>
    protected Quaternion GetHitRotationOfPointedObjectFinal()
    {
        Quaternion hitRotationResult = rot;
        foreach (GameObject controller in StaticGaze.GetAllGameObjectsWithArrow())
        {
            if (controller.GetComponent<ArrowControllerHandler>().pointerHitPosition != far)
            {
                hitRotationResult = controller.GetComponent<ArrowControllerHandler>().pointerHitRotation;
            }
        }
        return hitRotationResult;
    }

    public void SetTextOfGlobalGazingLabel()
    {
        if (sharingGlobal == true)
        {
            GetGlobalGazingLabelObject().GetComponent<TextMeshPro>().text = "Disable Gazing";
        }
        else
        {
            GetGlobalGazingLabelObject().GetComponent<TextMeshPro>().text = "Enable Gazing";
        }
    }

        protected string GetNameOfOwner(GameObject arrow)
        {
            return arrow.GetPhotonView().Owner.NickName;
        }

        protected Sprite GetIconForDevice(int device)
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