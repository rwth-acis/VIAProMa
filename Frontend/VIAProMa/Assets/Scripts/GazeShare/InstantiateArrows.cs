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
            if (controller.InputSource.SourceType == InputSourceType.Hand)
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
    private Vector3 targetOrigin;
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
    protected bool isUsingVive;
    protected int deviceUsed;
    protected int deviceUsedTarget;
    protected string textToShow;
    protected string targetTextToShow;
    [SerializeField] private Sprite hololensIcon;
    [SerializeField] private Sprite htcViveIcon;
    [HideInInspector] public bool sharingGlobal { get; private set; }
    // GetComponent variables
    private TextMeshPro globalGazingLabel;
    private Renderer rendererComponent;
    private TextMeshPro photonTextMeshPro;
    private SpriteRenderer photonSpriteRenderer;
    // Laser pointer and timer
    private LineRenderer lineRenderer;
    private float timer;

    public void Start()
    {
        StaticGaze.InstantiateSharing();
        InstantiateSharingGlobal();
        SetColorOfArrow();
        // GetComponent variables
        if (SceneManagerHelper.ActiveSceneName == "MainScene")
        {
            globalGazingLabel = GetGlobalGazingLabelObject().GetComponent<TextMeshPro>();
        }
        photonTextMeshPro = gameObject.GetComponentInChildren<TextMeshPro>();
        photonSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        // Setting up the laser pointer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
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
            // Laser functionality
            if (StaticGaze.GetIsUsingVive() == false) // HoloLens
            {
                stream.SendNext(MixedRealityToolkit.InputSystem.GazeProvider.GazeOrigin);
                stream.SendNext(-1.0f); // Negative time value for the HoloLens
            }
            else if (StaticGaze.GetIsUsingVive() == true)
            {
                stream.SendNext(RaycastVive.pointerOrigin);
                stream.SendNext(RaycastVive.timer);
            }

            stream.Serialize(ref deviceUsed);
            stream.Serialize(ref textToShow);
        }
        else
        {
            targetPosition = (Vector3)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();
            // Laser functionality
            targetOrigin = (Vector3)stream.ReceiveNext();
            timer = (float)stream.ReceiveNext();

            stream.Serialize(ref deviceUsedTarget);
            stream.Serialize(ref targetTextToShow);
        }
    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            MoveMyArrow();
            textToShow = photonView.Owner.NickName;
            if (SceneManagerHelper.ActiveSceneName != "GazeShareTest")
            {
                photonTextMeshPro.text = textToShow;
            }
            photonSpriteRenderer.sprite = GetIconForDevice(deviceUsed);
        }
        else
        {
            MoveOtherArrows();
            photonTextMeshPro.text = targetTextToShow;
            photonSpriteRenderer.sprite = GetIconForDevice(deviceUsedTarget);
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
            // Laser functionality
            if (RaycastVive.laserOn == true)
            {
                lineRenderer.SetPosition(0, MixedRealityToolkit.InputSystem.GazeProvider.GazeOrigin);
                lineRenderer.SetPosition(1, currentHitPosition);
                lineRenderer.enabled = true;
            }
        }
        else if (StaticGaze.GetIsUsingVive() == true && StaticGaze.sharing == true && sharingGlobal == true)
        {
            GameObject target = GameObject.FindGameObjectWithTag("cursor");
            transform.position = RaycastVive.pointerHitPosition; 
            transform.rotation = RaycastVive.pointerHitRotation; 
            deviceUsed = 2;
            // Laser functionality
            if (RaycastVive.timer != 0.0f && RaycastVive.laserOn == true)
            {
                lineRenderer.SetPosition(0, RaycastVive.pointerOrigin);
                lineRenderer.SetPosition(1, RaycastVive.pointerHitPosition);
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
        else
        {
            transform.position = far;
            transform.rotation = rot;
            // Laser functionality 
            lineRenderer.enabled = false;
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
            // Laser functionality
            if (targetPosition == far)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                if (deviceUsedTarget == 1 && RaycastVive.laserOn == true)
                {
                    lineRenderer.SetPosition(0, targetOrigin);
                    lineRenderer.SetPosition(1, targetPosition);
                    lineRenderer.enabled = true;
                }
                else if (deviceUsedTarget == 2 && timer != 0.0f && RaycastVive.laserOn == true)
                {
                    lineRenderer.SetPosition(0, targetOrigin);
                    lineRenderer.SetPosition(1, targetPosition);
                    lineRenderer.enabled = true;
                }
                else
                {
                    lineRenderer.enabled = false;
                }
            }
        }
        else
        {
            transform.position = far;
            transform.rotation = rot;
            // Laser functionality 
            lineRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Depending on the owner actor number this method gives the color of the arrow
    /// </summary>
    /// <param name="userID">A owner actor number of a photon view</param>
    /// <returns>The color of the arrow</returns>
    protected Color GetColor()
    {
        var hue = Random.Range(0f, 1f);
        return Color.HSVToRGB(hue, 1f, 1f);
    }

    /// <summary>
    /// Gets the renderer of the gameobject to change color ans set opacity
    /// </summary>
    protected void SetColorOfArrow()
    {
        Color generatedColor = GetColor();
        GetComponent<Renderer>().material.color = new Color(generatedColor.r, generatedColor.g, generatedColor.b, 0.3f);
    }

    protected GameObject GetGlobalGazingLabelObject()
    {
        GameObject globalGazinglabelObject = GameObject.Find("GlobalGazingLabel");
        return globalGazinglabelObject;
    }

    public void SetTextOfGlobalGazingLabel()
    {
        if (sharingGlobal == true)
        {
            globalGazingLabel.text = "Disable Gazing";
        }
        else
        {
            globalGazingLabel.text = "Enable Gazing";
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